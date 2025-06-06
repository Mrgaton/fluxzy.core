// Copyright 2021 - Haga Rakotoharivelo - https://github.com/haga-rak

using MessagePack;
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace Fluxzy.Clients.H11
{
    public enum WsMessageDirection
    {
        Sent = 1,
        Receive
    }

    [MessagePackObject]
    public class WsMessage
    {
        public WsMessage(int id)
        {
            Id = id;
        }

        [Key(0)]
        public int Id { get; set; }

        [Key(1)]
        public WsMessageDirection Direction { get; set; }

        [Key(2)]
        public WsOpCode OpCode { get; set; }

        [Key(3)]
        public long Length { get; set; }

        [Key(4)]
        public long WrittenLength { get; set; }

        [Key(5)]
        public byte[]? Data { get; set; }

        [Key(7)]
        public string? DataString { get; set; }

        [Key(8)]
        public DateTime MessageStart { get; set; }

        [Key(9)]
        public DateTime MessageEnd { get; set; }

        [Key(10)]
        public int FrameCount { get; set; }

        internal void ApplyXor(Span<byte> data, uint mask, int countIndex)
        {
            if (mask == 0)
                return;

            mask = RotateLeft(mask, countIndex % 4 * 8);
            
            Span<byte> maskData = stackalloc byte[4];

            for (var i = 0; i < data.Length;) {
                var currentBlock = data.Slice(i);

                if (currentBlock.Length >= 4) {
                    var val = BinaryPrimitives.ReadUInt32BigEndian(currentBlock);
                    var finalVal = val ^ mask;
                    BinaryPrimitives.WriteUInt32BigEndian(currentBlock, finalVal);
                    i += 4;

                    continue;
                }

                BinaryPrimitives.WriteUInt32BigEndian(maskData, mask);

                for (var j = 0; j < currentBlock.Length; j++) {
                    currentBlock[j] = (byte) (currentBlock[j] ^ maskData[j]);
                }

                break;
            }
        }

        internal async Task AddFrame(
            WsFrame wsFrame, int maxWsMessageLengthBuffered, PipeReader pipeReader,
            Func<int, Stream> outStream, CancellationToken token)
        {
            if (wsFrame.OpCode != 0) // We don't affect continuation frame
                OpCode = wsFrame.OpCode;

            if (wsFrame.FinalFragment && Length == 0 && wsFrame.PayloadLength < maxWsMessageLengthBuffered) {
                // Build direct buffer for message 

                var readResult = await pipeReader.ReadAtLeastAsync((int) wsFrame.PayloadLength, token).ConfigureAwait(false);

                // TODO optimize with stackalloc on sequence

                Data = readResult.Buffer.ToArray();

                ApplyXor(Data, wsFrame.MaskedPayload, 0);

                WrittenLength = Data.Length;

                pipeReader.AdvanceTo(readResult.Buffer.GetPosition(wsFrame.PayloadLength));
            }
            else {
                await using var stream = outStream(Id);
                var totalWr = 0;

                while (WrittenLength < wsFrame.PayloadLength) {
                    // Write into file 

                    if (!pipeReader.TryRead(out var readResult))
                        readResult = await pipeReader.ReadAsync().ConfigureAwait(false);

                    // readResult.Buffer.Slice()

                    var effectiveBufferLength =
                        (int) Math.Min(readResult.Buffer.Length, wsFrame.PayloadLength - WrittenLength);

                    var totalWriteInSequence = 0;

                    foreach (var sequence in readResult.Buffer) {
                        var memory = sequence.Slice(0,
                            Math.Min(sequence.Length, effectiveBufferLength - totalWriteInSequence));

                        if (wsFrame.MaskedPayload != 0) {
                            var copyBuffer = ArrayPool<byte>.Shared.Rent(memory.Length);

                            try {
                                memory.CopyTo(copyBuffer);

                                ApplyXor(new Span<byte>(copyBuffer, 0, memory.Length),
                                    wsFrame.MaskedPayload, totalWr % 4);

                                totalWr += memory.Length;

                                //  lengthShift = (lengthShift + memory.Length) % 4;

                                stream.Write(copyBuffer, 0, memory.Length);
                                stream.Flush();
                            }
                            finally {
                                ArrayPool<byte>.Shared.Return(copyBuffer);
                            }
                        }
                        else {
                            stream.Write(memory.Span);
                            stream.Flush();
                        }

                        totalWriteInSequence += memory.Length;
                    }

                    WrittenLength += effectiveBufferLength;

                    pipeReader.AdvanceTo(readResult.Buffer.GetPosition(effectiveBufferLength));
                }

                if (wsFrame.FinalFragment) {
                }
            }

            FrameCount++;
            Length += wsFrame.PayloadLength;
        }

        internal static uint RotateLeft(uint value, int count)
        {
            return (value << count) | (value >> (32 - count));
        }

        internal static uint RotateRight(uint value, int count)
        {
            return (value >> count) | (value << (32 - count));
        }
    }
}
