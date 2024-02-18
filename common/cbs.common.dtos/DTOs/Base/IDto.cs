using System.ComponentModel.DataAnnotations;

namespace cbs.common.dtos.DTOs.Base;

public interface IDto
{
    [Key]
    string Id { get; set; }
}