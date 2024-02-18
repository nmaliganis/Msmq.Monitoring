using System;
using System.ComponentModel.DataAnnotations;

namespace cbs.common.dtos.ResourceParameters.Messages;

public class CreateMessageResourceParameters
{
    [Required]
    public string Gender { get; set; }
    [Required]
    public string Firstname { get; set; }
    [Required]
    public string Lastname { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public int Calls { get; set; }
    [Required]
    public DateTime Dob { get; set; }
    [Required]
    public string Street { get; set; }
    [Required]
    public string City { get; set; }
    [Required]
    public string Country { get; set; }
    [Required]
    public string Postcode { get; set; }
}