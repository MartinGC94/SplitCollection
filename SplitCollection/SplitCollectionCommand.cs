using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace SplitCollection
{
    /// <summary>Splits the input collection into smaller arrays.</summary>
    [Cmdlet(VerbsCommon.Split, "Collection")]

    public sealed class SplitCollectionCommand : PSCmdlet
    {
        bool usePipeline = false;
        List<object> pipelineOutputList = new List<object>();

        #region parameters
        /// <summary>The input collection.</summary>
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public object[] InputObject { get; set; }

        /// <summary>The size of each array chunk.</summary>
        [Parameter(ParameterSetName = "SplitByChunkSize")]
        [ValidateRange(1, int.MaxValue)]
        public int ChunkSize { get; set; }

        /// <summary>The amount of parts to split to split the input object into.</summary>
        [Parameter(ParameterSetName = "SplitByAmountOfParts")]
        [ValidateRange(1, int.MaxValue)]
        public int AmountOfParts { get; set; }
        #endregion

        protected override void BeginProcessing()
        {
            if (MyInvocation.ExpectingInput == true)
            {
                usePipeline = true;
                if (AmountOfParts > 0)
                {
                    WriteWarning("When the \"AmountOfParts\" parameter is used inside a pipeline the rest of the pipeline is paused until all input data has been processed.");
                }
            }
        }
        /// <summary>Handles any pipeline input</summary>
        protected override void ProcessRecord()
        {
            if (usePipeline == true && ChunkSize > 0)
            {
                pipelineOutputList.Add(InputObject[0]);
                if (pipelineOutputList.Count == ChunkSize)
                {
                    WriteObject(pipelineOutputList.ToArray());
                    pipelineOutputList.Clear();
                }
            }
            else if (usePipeline == true && AmountOfParts > 0)
            {
                pipelineOutputList.Add(InputObject[0]);
            }
        }
        /// <summary>Outputs any remaining pipeline input, and handles the splitting for non pipeline input</summary>
        protected override void EndProcessing()
        {
            if (pipelineOutputList.Count > 0)
            {
                if (ChunkSize > 0)
                {
                    WriteObject(pipelineOutputList.ToArray());
                }
                else
                {
                    InputObject = pipelineOutputList.ToArray();
                    usePipeline = false;
                }
            }
            if (usePipeline == false)
            {
                Int32 InputObjectCount = InputObject.Length;
                Int32 SizeOfFinalChunk = 0;

                /// <summary>Finds the optimal chunksize based on the requested amount of parts, and if there is an uneven amount, the size of the final chunk.</summary>
                if (AmountOfParts > 0)
                {
                    if (AmountOfParts > InputObjectCount)
                    {
                        WriteWarning("The desired amount of parts is higher than the amount of objects in the collection.");
                    }
                    Double ChunkSizeBeforeRounding = (Convert.ToDouble(InputObjectCount) / Convert.ToDouble(AmountOfParts));
                    if ((int)ChunkSizeBeforeRounding == ChunkSizeBeforeRounding)
                    {
                        ChunkSize = Convert.ToInt32(ChunkSizeBeforeRounding);
                    }
                    else
                    {
                        Double ChunkSizeCeiling = Math.Ceiling(ChunkSizeBeforeRounding);
                        //If we round up, are there enough objects for 1 last chunk?
                        if (ChunkSizeCeiling * (AmountOfParts - 1) < InputObjectCount)
                        {
                            ChunkSize = Convert.ToInt32(ChunkSizeCeiling);
                        }
                        else
                        {
                            Double ChunkSizeFloor = Math.Floor(ChunkSizeBeforeRounding);
                            ChunkSize = Convert.ToInt32(ChunkSizeFloor);
                        }
                        SizeOfFinalChunk = InputObjectCount - (ChunkSize * (AmountOfParts - 1));
                    }
                }

                if (ChunkSize > InputObjectCount)
                {
                    WriteWarning("The chunksize is higher than the amount of objects in the collection.");
                }
                /// <summary>Splits the inputobject into smaller chunks and writes them to the pipeline.</summary>
                Int32 Skip = 0;
                while (Skip < InputObjectCount)
                {
                    //Is the size of the final chunk special, and is this the final chunk?
                    if (SizeOfFinalChunk > 0 && InputObjectCount == Skip + SizeOfFinalChunk)
                    {
                        ChunkSize = SizeOfFinalChunk;
                    }
                    //Are there enough objects to fill up a full chunk?
                    if (InputObjectCount >= Skip + ChunkSize)
                    {
                        Array TempArray = Array.CreateInstance(typeof(object), ChunkSize);
                        Array.Copy(InputObject, Skip, TempArray, 0, ChunkSize);
                        WriteObject(TempArray);
                    }
                    else
                    {
                        Array TempArray = Array.CreateInstance(typeof(object), InputObjectCount - Skip);
                        Array.Copy(InputObject, Skip, TempArray, 0, InputObjectCount - Skip);
                        WriteObject(TempArray);
                    }
                    Skip += ChunkSize;
                }
            }
        }
    }
}
