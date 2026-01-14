namespace DRSSoftware.EnigmaMachine.ViewModels;

using System.Collections.ObjectModel;
using System.Windows.Input;
using DRSSoftware.EnigmaMachine.Commands;

/// <summary>
/// View model used for design-time data binding in the ConfigurationDialogView.
/// </summary>
internal sealed class ConfigurationDialogViewModelDT : ViewModelBase, IConfigurationDialogViewModel
{
    private readonly ReadOnlyCollection<int> _availableIndexValues = new List<int>([.. Enumerable.Range(0, 95)]).AsReadOnly();
    private readonly ReadOnlyCollection<int> _availableRotorCounts = new List<int>([.. Enumerable.Range(3, 8)]).AsReadOnly();
    private bool _closeTrigger;
    private bool _isAutoIndexesSelected;
    private bool _isAutoRotorsSelected;
    private bool _isAutoSeedSelected;
    private bool _isManualIndexesSelected = true;
    private bool _isManualRotorsSelected = true;
    private bool _isManualSeedSelected = true;
    private bool _isRotor4Visible = true;
    private bool _isRotor5Visible = true;
    private bool _isRotor6Visible = true;
    private bool _isRotor7Visible = true;
    private bool _isRotor8Visible = true;
    private int _reflectorIndex = 43;
    private int _rotorIndex1 = 13;
    private int _rotorIndex2 = 27;
    private int _rotorIndex3 = 65;
    private int _rotorIndex4 = 91;
    private int _rotorIndex5 = 3;
    private int _rotorIndex6 = 61;
    private int _rotorIndex7 = 12;
    private int _rotorIndex8 = 88;
    private string _seedValue = "Design time seed value.";
    private int _selectedRotorCount = 8;

    /// <summary>
    /// Gets the command used for accepting the input string.
    /// </summary>
    public ICommand AcceptCommand => new RelayCommand(static _ => { }, static _ => true);

    /// <summary>
    /// Gets a list of available rotor index values.
    /// </summary>
    public ReadOnlyCollection<int> AvailableIndexValues => _availableIndexValues;

    /// <summary>
    /// Gets a list of available rotor counts.
    /// </summary>
    public ReadOnlyCollection<int> AvailableRotorCounts => _availableRotorCounts;

    /// <summary>
    /// Gets or sets a value indicating whether the associated view should be closed.
    /// </summary>
    public bool CloseTrigger
    {
        get => _closeTrigger;
        set
        {
            if (_closeTrigger != value)
            {
                _closeTrigger = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether automatic rotor indexes selection is enabled.
    /// </summary>
    public bool IsAutoIndexesSelected
    {
        get => _isAutoIndexesSelected;
        set
        {
            if (_isAutoIndexesSelected != value)
            {
                _isAutoIndexesSelected = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether automatic rotor count selection is enabled.
    /// </summary>
    public bool IsAutoRotorsSelected
    {
        get => _isAutoRotorsSelected;
        set
        {
            if (!_isAutoRotorsSelected != value)
            {
                _isAutoRotorsSelected = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not the seed value is automatically generated.
    /// </summary>
    public bool IsAutoSeedSelected
    {
        get => _isAutoSeedSelected;
        set
        {
            if (_isAutoSeedSelected != value)
            {
                _isAutoSeedSelected = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether rotor indexes will be manually set.
    /// </summary>
    public bool IsManualIndexesSelected
    {
        get => _isManualIndexesSelected;
        set
        {
            if (_isManualIndexesSelected != value)
            {
                _isManualIndexesSelected = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the rotor count will be manually set.
    /// </summary>
    public bool IsManualRotorsSelected
    {
        get => _isManualRotorsSelected;
        set
        {
            if (_isManualRotorsSelected != value)
            {
                _isManualRotorsSelected = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not the seed value will be manually entered.
    /// </summary>
    public bool IsManualSeedSelected
    {
        get => _isManualSeedSelected;
        set
        {
            if (_isManualSeedSelected != value)
            {
                _isManualSeedSelected = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not rotor #4 is visible.
    /// </summary>
    public bool IsRotor4Visible
    {
        get => _isRotor4Visible;
        set
        {
            if (_isRotor4Visible != value)
            {
                _isRotor4Visible = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not rotor #5 is visible.
    /// </summary>
    public bool IsRotor5Visible
    {
        get => _isRotor5Visible;
        set
        {
            if (_isRotor5Visible != value)
            {
                _isRotor5Visible = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not rotor #6 is visible.
    /// </summary>
    public bool IsRotor6Visible
    {
        get => _isRotor6Visible;
        set
        {
            if (_isRotor6Visible != value)
            {
                _isRotor6Visible = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not rotor #7 is visible.
    /// </summary>
    public bool IsRotor7Visible
    {
        get => _isRotor7Visible;
        set
        {
            if (_isRotor7Visible != value)
            {
                _isRotor7Visible = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not rotor #8 is visible.
    /// </summary>
    public bool IsRotor8Visible
    {
        get => _isRotor8Visible;
        set
        {
            if (_isRotor8Visible != value)
            {
                _isRotor8Visible = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected index value of the reflector.
    /// </summary>
    public int ReflectorIndex
    {
        get => _reflectorIndex;
        set
        {
            if (_reflectorIndex != value)
            {
                _reflectorIndex = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected index value of rotor #1.
    /// </summary>
    public int RotorIndex1
    {
        get => _rotorIndex1;
        set
        {
            if (_rotorIndex1 != value)
            {
                _rotorIndex1 = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected index value of rotor #2.
    /// </summary>
    public int RotorIndex2
    {
        get => _rotorIndex2;
        set
        {
            if (_rotorIndex2 != value)
            {
                _rotorIndex2 = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected index value of rotor #3.
    /// </summary>
    public int RotorIndex3
    {
        get => _rotorIndex3;
        set
        {
            if (_rotorIndex3 != value)
            {
                _rotorIndex3 = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected index value of rotor #4.
    /// </summary>
    public int RotorIndex4
    {
        get => _rotorIndex4;
        set
        {
            if (_rotorIndex4 != value)
            {
                _rotorIndex4 = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected index value of rotor #5.
    /// </summary>
    public int RotorIndex5
    {
        get => _rotorIndex5;
        set
        {
            if (_rotorIndex5 != value)
            {
                _rotorIndex5 = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected index value of rotor #6.
    /// </summary>
    public int RotorIndex6
    {
        get => _rotorIndex6;
        set
        {
            if (_rotorIndex6 != value)
            {
                _rotorIndex6 = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected index value of rotor #7.
    /// </summary>
    public int RotorIndex7
    {
        get => _rotorIndex7;
        set
        {
            if (_rotorIndex7 != value)
            {
                _rotorIndex7 = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected index value of rotor #8.
    /// </summary>
    public int RotorIndex8
    {
        get => _rotorIndex8;
        set
        {
            if (_rotorIndex8 != value)
            {
                _rotorIndex8 = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the seed value used for randomizing the wiring of the reflector and rotors.
    /// </summary>
    public string SeedValue
    {
        get => _seedValue;
        set
        {
            if (!_seedValue.Equals(value, StringComparison.OrdinalIgnoreCase))
            {
                _seedValue = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the currently selected rotor count.
    /// </summary>
    public int SelectedRotorCount
    {
        get => _selectedRotorCount;
        set
        {
            if (_selectedRotorCount != value)
            {
                _selectedRotorCount = value;
                OnPropertyChanged();
            }
        }
    }
}