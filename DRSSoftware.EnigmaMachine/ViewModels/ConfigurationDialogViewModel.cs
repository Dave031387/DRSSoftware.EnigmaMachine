namespace DRSSoftware.EnigmaMachine.ViewModels;

using System.Collections.ObjectModel;
using System.Windows.Input;
using DRSSoftware.EnigmaMachine.Commands;
using DRSSoftware.EnigmaMachine.Utility;

/// <summary>
/// Represents the view model for the Enigma machine configuration dialog.
/// </summary>
internal sealed class ConfigurationDialogViewModel : ViewModelBase, IConfigurationDialogViewModel
{
    /// <summary>
    /// A value that is one higher than the maximum length of an automatically generated seed value.
    /// </summary>
    private const int AutoSeedMaxLength = 65;

    /// <summary>
    /// A value equal to the minimum length of an automatically generated seed value.
    /// </summary>
    private const int AutoSeedMinLength = 32;

    /// <summary>
    /// A value that is one higher than the maximum rotor index value.
    /// </summary>
    private const int IndexValueLimit = MaxIndexValue + 1;

    /// <summary>
    /// A value equal to the maximum rotor index value.
    /// </summary>
    private const int MaxIndexValue = 95;

    /// <summary>
    /// A value equal to the maximum number of rotors supported by the Enigma machine.
    /// </summary>
    private const int MaxRotorCount = 8;

    /// <summary>
    /// A value equal to the minimum rotor index value.
    /// </summary>
    private const int MinIndexValue = 0;

    /// <summary>
    /// A value equal to the minimum number of rotors supported by the Enigma machine.
    /// </summary>
    private const int MinRotorCount = 3;

    /// <summary>
    /// A value that is one higher than the maximum number of rotors supported by the Enigma
    /// machine.
    /// </summary>
    private const int RotorCountLimit = MaxRotorCount + 1;

    /// <summary>
    /// A value equal to the number of available rotor count options.
    /// </summary>
    private const int RotorOptionsLimit = RotorCountLimit - MinRotorCount;

    /// <summary>
    /// Holds a reference to the secure random number generator.
    /// </summary>
    private readonly ISecureNumberGenerator _numberGenerator;

    /// <summary>
    /// The backing field for the CloseTrigger property.
    /// </summary>
    private bool _closeTrigger;

    /// <summary>
    /// The backing field for the IsAutoIndexesSelected property.
    /// </summary>
    private bool _isAutoIndexesSelected;

    /// <summary>
    /// The backing field for the IsAutoRotorsSelected property.
    /// </summary>
    private bool _isAutoRotorsSelected;

    /// <summary>
    /// The backing field for the IsAutoSeedSelected property.
    /// </summary>
    private bool _isAutoSeedSelected;

    /// <summary>
    /// The backing field for the IsManualIndexesSelected property.
    /// </summary>
    private bool _isManualIndexesSelected = true;

    /// <summary>
    /// The backing field for the IsManualRotorsSelected property.
    /// </summary>
    private bool _isManualRotorsSelected = true;

    /// <summary>
    /// The backing field for the IsManualSeedSelected property.
    /// </summary>
    private bool _isManualSeedSelected = true;

    /// <summary>
    /// The backing field for the IsRotor4Visible property.
    /// </summary>
    private bool _isRotor4Visible;

    /// <summary>
    /// The backing field for the IsRotor5Visible property.
    /// </summary>
    private bool _isRotor5Visible;

    /// <summary>
    /// The backing field for the IsRotor6Visible property.
    /// </summary>
    private bool _isRotor6Visible;

    /// <summary>
    /// The backing field for the IsRotor7Visible property.
    /// </summary>
    private bool _isRotor7Visible;

    /// <summary>
    /// The backing field for the IsRotor8Visible property.
    /// </summary>
    private bool _isRotor8Visible;

    /// <summary>
    /// The backing field for the ReflectorIndex property.
    /// </summary>
    private int _reflectorIndex;

    /// <summary>
    /// The backing field for the RotorIndex1 property.
    /// </summary>
    private int _rotorIndex1;

    /// <summary>
    /// The backing field for the RotorIndex2 property.
    /// </summary>
    private int _rotorIndex2;

    /// <summary>
    /// The backing field for the RotorIndex3 property.
    /// </summary>
    private int _rotorIndex3;

    /// <summary>
    /// The backing field for the RotorIndex4 property.
    /// </summary>
    private int _rotorIndex4;

    /// <summary>
    /// The backing field for the RotorIndex5 property.
    /// </summary>
    private int _rotorIndex5;

    /// <summary>
    /// The backing field for the RotorIndex6 property.
    /// </summary>
    private int _rotorIndex6;

    /// <summary>
    /// The backing field for the RotorIndex7 property.
    /// </summary>
    private int _rotorIndex7;

    /// <summary>
    /// The backing field for the RotorIndex8 property.
    /// </summary>
    private int _rotorIndex8;

    /// <summary>
    /// The backing field for the SeedValue property.
    /// </summary>
    private string _seedValue = string.Empty;

    /// <summary>
    /// The backing field for the SelectedRotorCount property.
    /// </summary>
    private int _selectedRotorCount = 3;

    /// <summary>
    /// The backing field for the UseEmbeddedConfiguration property.
    /// </summary>
    private bool _useEmbeddedConfiguration;

    /// <summary>
    /// Creates a new instance of the <see cref="ConfigurationDialogViewModel" /> class.
    /// </summary>
    public ConfigurationDialogViewModel(ISecureNumberGenerator numberGenerator)
    {
        AcceptCommand = new RelayCommand(_ => Accept(), _ => IsValidConfiguration);
        CancelCommand = new RelayCommand(_ => Cancel(), _ => true);
        _numberGenerator = numberGenerator;
    }

    /// <summary>
    /// Gets an ICommand used for accepting the user-specified configuration settings and closing
    /// the associated dialog.
    /// </summary>
    public ICommand AcceptCommand
    {
        get;
    }

    /// <summary>
    /// Gets a list of available rotor index values.
    /// </summary>
    public ReadOnlyCollection<int> AvailableIndexValues
    {
        get;
    } = new List<int>([.. Enumerable.Range(0, IndexValueLimit)]).AsReadOnly();

    /// <summary>
    /// Gets a list of available rotor counts.
    /// </summary>
    public ReadOnlyCollection<int> AvailableRotorCounts
    {
        get;
    } = new List<int>([.. Enumerable.Range(MinRotorCount, RotorOptionsLimit)]).AsReadOnly();

    /// <summary>
    /// Gets an ICommand used for discarding the user-specified configuration settings and closing
    /// the associated dialog.
    /// </summary>
    public ICommand CancelCommand
    {
        get;
    }

    /// <summary>
    /// Gets or sets a value used for indicating whether the associated view should be closed.
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
    /// Gets the user-specified Enigma machine configuration.
    /// </summary>
    public EnigmaConfiguration EnigmaConfiguration
    {
        get;
        private set;
    } = new();

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

                if (_isAutoIndexesSelected)
                {
                    GenerateRandomIndexValues();
                }

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
            if (_isAutoRotorsSelected != value)
            {
                _isAutoRotorsSelected = value;

                if (_isAutoRotorsSelected)
                {
                    GenerateRandomRotorCount();
                }

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

                if (_isAutoSeedSelected)
                {
                    GenerateRandomSeedValue();
                }

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
            if (_seedValue != value)
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
                ShowVisibleRotors();
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not the Enigma machine configuration should be
    /// embedded into the encrypted text file.
    /// </summary>
    public bool UseEmbeddedConfiguration
    {
        get => _useEmbeddedConfiguration;
        set
        {
            if (_useEmbeddedConfiguration != value)
            {
                _useEmbeddedConfiguration = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether the current configuration is valid based on the seed value.
    /// </summary>
    private bool IsValidConfiguration => !string.IsNullOrWhiteSpace(_seedValue) && _seedValue.Length > 9;

    /// <summary>
    /// Initializes the view model with the current Enigma machine configuration.
    /// </summary>
    /// <param name="enigmaConfiguration">
    /// An <see cref="Utility.EnigmaConfiguration" /> object containing the current configuration of
    /// the Enigma machine.
    /// </param>
    public void Initialize(EnigmaConfiguration enigmaConfiguration)
    {
        EnigmaConfiguration = enigmaConfiguration;
        ReflectorIndex = EnigmaConfiguration.ReflectorIndex;
        RotorIndex1 = EnigmaConfiguration.RotorIndex1;
        RotorIndex2 = EnigmaConfiguration.RotorIndex2;
        RotorIndex3 = EnigmaConfiguration.RotorIndex3;
        RotorIndex4 = EnigmaConfiguration.RotorIndex4;
        RotorIndex5 = EnigmaConfiguration.RotorIndex5;
        RotorIndex6 = EnigmaConfiguration.RotorIndex6;
        RotorIndex7 = EnigmaConfiguration.RotorIndex7;
        RotorIndex8 = EnigmaConfiguration.RotorIndex8;
        SelectedRotorCount = EnigmaConfiguration.NumberOfRotors;
        SeedValue = EnigmaConfiguration.SeedValue;
        UseEmbeddedConfiguration = EnigmaConfiguration.UseEmbeddedConfiguration;
        ShowVisibleRotors();
    }

    /// <summary>
    /// Saves the configuration and closes the associated configuration dialog by setting the
    /// CloseTrigger to true.
    /// </summary>
    private void Accept()
    {
        EnigmaConfiguration.ReflectorIndex = ReflectorIndex;
        EnigmaConfiguration.RotorIndex1 = RotorIndex1;
        EnigmaConfiguration.RotorIndex2 = RotorIndex2;
        EnigmaConfiguration.RotorIndex3 = RotorIndex3;
        EnigmaConfiguration.RotorIndex4 = RotorIndex4;
        EnigmaConfiguration.RotorIndex5 = RotorIndex5;
        EnigmaConfiguration.RotorIndex6 = RotorIndex6;
        EnigmaConfiguration.RotorIndex7 = RotorIndex7;
        EnigmaConfiguration.RotorIndex8 = RotorIndex8;
        EnigmaConfiguration.NumberOfRotors = SelectedRotorCount;
        EnigmaConfiguration.SeedValue = SeedValue;
        EnigmaConfiguration.UseEmbeddedConfiguration = UseEmbeddedConfiguration;
        CloseTrigger = true;
    }

    /// <summary>
    /// Discards any changes and closes the associated configuration dialog by setting the
    /// CloseTrigger to true.
    /// </summary>
    private void Cancel() => CloseTrigger = true;

    /// <summary>
    /// Generate random index values for the reflector and rotors.
    /// </summary>
    private void GenerateRandomIndexValues()
    {
        ReflectorIndex = _numberGenerator.GetNext(MinIndexValue, IndexValueLimit);
        RotorIndex1 = _numberGenerator.GetNext(MinIndexValue, IndexValueLimit);
        RotorIndex2 = _numberGenerator.GetNext(MinIndexValue, IndexValueLimit);
        RotorIndex3 = _numberGenerator.GetNext(MinIndexValue, IndexValueLimit);
        RotorIndex4 = _numberGenerator.GetNext(MinIndexValue, IndexValueLimit);
        RotorIndex5 = _numberGenerator.GetNext(MinIndexValue, IndexValueLimit);
        RotorIndex6 = _numberGenerator.GetNext(MinIndexValue, IndexValueLimit);
        RotorIndex7 = _numberGenerator.GetNext(MinIndexValue, IndexValueLimit);
        RotorIndex8 = _numberGenerator.GetNext(MinIndexValue, IndexValueLimit);
    }

    /// <summary>
    /// Pick a random rotor count within the valid range.
    /// </summary>
    private void GenerateRandomRotorCount()
        => SelectedRotorCount = _numberGenerator.GetNext(MinRotorCount, RotorCountLimit);

    /// <summary>
    /// Generate a random seed string value having a random length between 32 and 64.
    /// </summary>
    private void GenerateRandomSeedValue()
    {
        int seedLength = _numberGenerator.GetNext(AutoSeedMinLength, AutoSeedMaxLength);
        char[] seedChars = new char[seedLength];

        for (int i = 0; i < seedLength; i++)
        {
            seedChars[i] = (char)(_numberGenerator.GetNext(MinIndexValue, MaxIndexValue) + ' ');
        }

        SeedValue = new string(seedChars);
    }

    /// <summary>
    /// Show or hide rotor index value selectors based on the selected rotor count.
    /// </summary>
    private void ShowVisibleRotors()
    {
        IsRotor4Visible = _selectedRotorCount > 3;
        IsRotor5Visible = _selectedRotorCount > 4;
        IsRotor6Visible = _selectedRotorCount > 5;
        IsRotor7Visible = _selectedRotorCount > 6;
        IsRotor8Visible = _selectedRotorCount > 7;
    }
}