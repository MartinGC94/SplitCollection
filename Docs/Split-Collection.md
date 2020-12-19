---
external help file: SplitCollection.dll-Help.xml
Module Name: SplitCollection
online version:
schema: 2.0.0
---

# Split-Collection

## SYNOPSIS
Splits collections into smaller collections.

## SYNTAX

### SplitByChunkSize
```
Split-Collection [-InputObject] <Object[]> [-ChunkSize <Int32>] [<CommonParameters>]
```

### SplitByAmountOfParts
```
Split-Collection [-InputObject] <Object[]> [-AmountOfParts <Int32>] [<CommonParameters>]
```

## DESCRIPTION
This cmdlet splits collections (arrays, lists, etc.) into smaller arrays.  
Collections can be split in 2 ways:

ChunkSize (each sub array will have the user specified size.)  
AmountOfParts (the input objects will be divided evenly between X amount of sub arrays.)

The cmdlet generally doesn't throw errors, but it will write warnings if there aren't enough input objects for the specified sizes.
## EXAMPLES

### Example 1 Splitting objects evenly for jobs to process
```powershell
PS C:\> Split-Collection -InputObject (Get-Content -Path C:\FolderList.txt) -AmountOfParts 4 | ForEach {Start-Job -ScriptBlock {Invoke-Scan $using:_}}
```

This example divides the paths specified in C:\FolderList.txt evenly into 4 chunks. Each chunk is then used to start a job that processes these paths.

## PARAMETERS

### -InputObject
The collection to split into smaller arrays.

```yaml
Type: Object[]
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -ChunkSize
Specifies how big each sub array will be.

```yaml
Type: Int32
Parameter Sets: SplitByChunkSize
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -AmountOfParts
Specifies how many sub arrays will be created. The input collection is divided evenly between each of the new arrays.

```yaml
Type: Int32
Parameter Sets: SplitByAmountOfParts
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.Object[]

## OUTPUTS

### System.Object[]

## NOTES

## RELATED LINKS
