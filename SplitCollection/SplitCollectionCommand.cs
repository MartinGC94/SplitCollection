using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace SplitCollection
{
    /// <summary>Splits the input collection into smaller arrays.</summary>
    [Cmdlet(VerbsCommon.Split, "Collection", DefaultParameterSetName = "SplitByChunkSize")]
    [OutputType(typeof(object[]))]

    public sealed class SplitCollectionCommand : PSCmdlet
    {
        private bool _InputFromPipeline = false;
        private readonly List<object> _PipelineOutputList = new List<object>();

        #region parameters
        /// <summary>The input collection.</summary>
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public object[] InputObject { get; set; }

        /// <summary>The size of each array chunk.</summary>
        [Parameter(Mandatory = true,ParameterSetName = "SplitByChunkSize")]
        [ValidateRange(1, int.MaxValue)]
        public int ChunkSize { get; set; }

        /// <summary>The amount of parts to split to split the input object into.</summary>
        [Parameter(Mandatory = true, ParameterSetName = "SplitByAmountOfParts")]
        [ValidateRange(1, int.MaxValue)]
        public int AmountOfParts { get; set; }
        #endregion

        protected override void BeginProcessing()
        {
            _InputFromPipeline = MyInvocation.ExpectingInput;
        }
        /// <summary>Handles any pipeline input</summary>
        protected override void ProcessRecord()
        {
            if (_InputFromPipeline == true)
            {
                _PipelineOutputList.Add(InputObject[0]);
                if (_PipelineOutputList.Count == ChunkSize)
                {
                    WriteObject(_PipelineOutputList.ToArray());
                    _PipelineOutputList.Clear();
                }
            }
        }
        /// <summary>Outputs any remaining pipeline input, and handles the splitting for non pipeline input</summary>
        protected override void EndProcessing()
        {
            if (_PipelineOutputList.Count > 0)
            {
                if (ChunkSize > 0)
                {
                    WriteObject(_PipelineOutputList.ToArray());
                }
                else
                {
                    InputObject = _PipelineOutputList.ToArray();
                    _InputFromPipeline = false;
                }
            }
            if (_InputFromPipeline == false)
            {
                int inputObjectCount = InputObject.Length;
                int sizeOfFinalChunk = 0;

                /// <summary>Finds the optimal chunksize based on the requested amount of parts, and if there is an uneven amount, the size of the final chunk.</summary>
                if (AmountOfParts > 0)
                {
                    if (AmountOfParts > inputObjectCount)
                    {
                        WriteWarning("The desired amount of parts is higher than the amount of objects in the collection.");
                    }

                    double chunkSizeBeforeRounding = (double)inputObjectCount / AmountOfParts;
                    if (chunkSizeBeforeRounding % 1 == 0)
                    {
                        ChunkSize = (int)chunkSizeBeforeRounding;
                    }
                    else
                    {
                        double chunkSizeCeiling = Math.Ceiling(chunkSizeBeforeRounding);
                        //If we round up, are there enough objects for 1 last chunk?
                        if (chunkSizeBeforeRounding < 1 || chunkSizeCeiling * (AmountOfParts - 1) < inputObjectCount)
                        {
                            ChunkSize = (int)chunkSizeCeiling;
                        }
                        else
                        {
                            double ChunkSizeFloor = Math.Floor(chunkSizeBeforeRounding);
                            ChunkSize = (int)ChunkSizeFloor;
                        }
                        sizeOfFinalChunk = inputObjectCount - (ChunkSize * (AmountOfParts - 1));
                    }
                }

                if (ChunkSize > inputObjectCount)
                {
                    WriteWarning("The chunksize is higher than the amount of objects in the collection.");
                }
                /// <summary>Splits the inputobject into smaller chunks and writes them to the pipeline.</summary>
                int skip = 0;
                object[] tempArray;
                while (skip < inputObjectCount)
                {
                    //Is the size of the final chunk special, and is this the final chunk?
                    if (sizeOfFinalChunk > 0 && inputObjectCount == skip + sizeOfFinalChunk)
                    {
                        ChunkSize = sizeOfFinalChunk;
                    }
                    //Are there enough objects to fill up a full chunk?
                    if (inputObjectCount >= skip + ChunkSize)
                    {
                        tempArray = new object[ChunkSize];
                        Array.Copy(InputObject, skip, tempArray, 0, ChunkSize);
                    }
                    else
                    {
                        tempArray = new object[inputObjectCount - skip];
                        Array.Copy(InputObject, skip, tempArray, 0, inputObjectCount - skip);
                    }
                    WriteObject(tempArray);
                    skip += ChunkSize;
                }
            }
        }
    }
}