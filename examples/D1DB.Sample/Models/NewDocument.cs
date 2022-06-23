// Copyright 2020-2022 CYBERCRYPT

using System.ComponentModel.DataAnnotations.Schema;
using CyberCrypt.D1.EntityFramework;

namespace D1DB.Sample.Models;

public class NewDocument
{
    public string? Data { get; set; }

    public string? AssociatedData { get; set; }
}
