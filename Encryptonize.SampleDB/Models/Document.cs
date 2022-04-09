// Copyright 2020-2022 CYBERCRYPT

using System.ComponentModel.DataAnnotations.Schema;
using Encryptonize.EntityFramework;

namespace EncryptonizeDBSample.Models;

public class Document
{
    public int Id { get; set; }

    [Confidential]
    public string? Data { get; set; }

    public string? AssociatedData { get; set; }
}
