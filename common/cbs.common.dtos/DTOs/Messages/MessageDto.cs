using System;
using System.ComponentModel.DataAnnotations;
using cbs.common.dtos.DTOs.Base;

namespace cbs.common.dtos.DTOs.Messages;

public class MessageDto : IDto
{
    [Key]
    public string Id { get; set; }
    
    public string Gender { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string Title { get; set; }
    public int Calls { get; set; }
    public DateTime Dob { get; set; }
    public string Nationality { get; set; }
}