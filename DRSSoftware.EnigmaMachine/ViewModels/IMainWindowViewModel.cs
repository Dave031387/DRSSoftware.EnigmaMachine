namespace DRSSoftware.EnigmaMachine.ViewModels;

using System.Windows.Input;

internal interface IMainWindowViewModel
{
    public ICommand CloakCommand
    {
        get;
    }

    public ICommand DecloakCommand
    {
        get;
    }

    ICommand EncryptCommand
    {
        get;
    }

    public string InputText
    {
        get; set;
    }

    public ICommand LoadCommand
    {
        get;
    }

    public string OutputText
    {
        get;
        set;
    }

    public ICommand ResetCommand
    {
        get;
    }

    public ICommand SaveCommand
    {
        get;
    }
}