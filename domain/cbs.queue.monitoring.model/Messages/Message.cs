using System;
using cbs.common.infrastructure;

namespace cbs.queue.monitoring.model.Messages;

public class Message : EntityBase<string>
{
    public Message()
    {
        OnCreate();
    }

    private void OnCreate()
    {
        this.Id = Guid.NewGuid().ToString();
    }

    public string Gender { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string Title { get; set; }
    public int Calls { get; set; }
    public DateTime Dob { get; set; }
    public string Nationality { get; set; }
    protected override void Validate()
    {
        
    }

    public void InjectWithValues(string gender, string firstname, string lastname, DateTime dob, int calls, string title, string street, string city, string postcode, string country)
    {
        this.Gender = gender;
        this.Firstname = firstname;
        this.Lastname = lastname;
        this.Dob = dob;
        this.Calls = calls;
        this.Title = title;
    }
}