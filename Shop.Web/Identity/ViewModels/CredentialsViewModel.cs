using FluentValidation.Attributes;

[Validator(typeof(CredentialsViewModelValidator))]
public class CredentialsViewModel
{
    public string UserName { get; set; }
    public string Password { get; set; }
}