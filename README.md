# SplitCollection
SplitCollection is a Powershell module containing a single cmdlet: "Split-Collection".
This cmdlet can be used to split collections (Arrays, lists, etc.) into smaller parts, either by chunksize or by amount of parts.
Examples where this might be useful could be splitting up large CSV files that are too big to be opened by certain programs, splitting large files that are too big for fat32 by reading the bytes and writing them to smaller files, splitting data for parallel processing with Powershell jobs or runspaces.
