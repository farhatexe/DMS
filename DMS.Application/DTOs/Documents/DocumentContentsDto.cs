﻿using DMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DMS.Application.DTOs.Documents
{
  public class DocumentContentsDto
  {
    public string Title { get; set; }
    public string Body { get; set; }
    public string Message { get; set; }
  }
}
