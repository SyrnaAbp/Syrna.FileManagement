﻿using System;

namespace Syrna.FileManagement.Files.Dtos;

[Serializable]
public class FileLocationDto
{
    public Guid Id { get; set; }

    public FileLocationModel Location { get; set; }
}