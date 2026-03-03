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
    /// A value equal to the minimum length of an automatically generated seed value.
    /// </summary>
    private const int AutoSeedMinLength = 32;

    /// <summary>
    /// Holds a reference to the secure random number generator.
    /// </summary>
    private readonly ISecureNumberGenerator _numberGenerator;

    /// <summary>
    /// Creates a new instance of the <see cref="ConfigurationDialogViewModel" /> class.
    /// </summary>
    public ConfigurationDialogViewModel(ISecureNumberGenerator numberGenerator)
    {
        AcceptCommand = new RelayCommand(_ => Accept(), _ => CanAccept);
        CancelCommand = new RelayCommand(_ => Cancel(), _ => true);
        ClearCommand = new RelayCommand(_ => Clear(), _ => CanClear);
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
    } = new List<int>([.. Enumerable.Range(MinValue, MaxValue + 1)]).AsReadOnly();

    /// <summary>
    /// Gets a list of available rotor counts.
    /// </summary>
    public ReadOnlyCollection<int> AvailableRotorCounts
    {
        get;
    } = new List<int>([.. Enumerable.Range(MinRotorCount, MaxRotorCount - MinRotorCount + 1)]).AsReadOnly();

    /// <summary>
    /// Gets an ICommand used for discarding the user-specified configuration settings and closing
    /// the associated dialog.
    /// </summary>
    public ICommand CancelCommand
    {
        get;
    }

    /// <summary>
    /// Gets an ICommand used for discarding the user-specified configuration settings and closing
    /// the associated dialog.
    /// </summary>
    public ICommand ClearCommand
    {
        get;
    }

    /// <summary>
    /// Gets or sets a value used for indicating whether the associated view should be closed.
    /// </summary>
    public bool CloseTrigger
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
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
        get;
        set
        {
            if (field != value)
            {
                field = value;

                if (field)
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
        get;
        set
        {
            if (field != value)
            {
                field = value;

                if (field)
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
        get;
        set
        {
            if (field != value)
            {
                field = value;

                if (field)
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
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the rotor count will be manually set.
    /// </summary>
    public bool IsManualRotorsSelected
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    } = true;

    /// <summary>
    /// Gets or sets a value indicating whether or not the seed value will be manually entered.
    /// </summary>
    public bool IsManualSeedSelected
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    } = true;

    /// <summary>
    /// Gets or sets a value indicating whether or not rotor #4 is visible.
    /// </summary>
    public bool IsRotor4Visible
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not rotor #5 is visible.
    /// </summary>
    public bool IsRotor5Visible
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not rotor #6 is visible.
    /// </summary>
    public bool IsRotor6Visible
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not rotor #7 is visible.
    /// </summary>
    public bool IsRotor7Visible
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not rotor #8 is visible.
    /// </summary>
    public bool IsRotor8Visible
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets the maximum seed value length.
    /// </summary>
    public int MaxSeedValueLength
    {
        get;
    } = MaxSeedLength;

    /// <summary>
    /// Gets the minimum seed value length.
    /// </summary>
    public int MinSeedValueLength
    {
        get;
    } = MinStringLength;

    /// <summary>
    /// Gets or sets the selected index value of the reflector.
    /// </summary>
    public int ReflectorIndex
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected index value of rotor #1.
    /// </summary>
    public int RotorIndex1
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected index value of rotor #2.
    /// </summary>
    public int RotorIndex2
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected index value of rotor #3.
    /// </summary>
    public int RotorIndex3
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected index value of rotor #4.
    /// </summary>
    public int RotorIndex4
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected index value of rotor #5.
    /// </summary>
    public int RotorIndex5
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected index value of rotor #6.
    /// </summary>
    public int RotorIndex6
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected index value of rotor #7.
    /// </summary>
    public int RotorIndex7
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected index value of rotor #8.
    /// </summary>
    public int RotorIndex8
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the seed value used for randomizing the wiring of the reflector and rotors.
    /// </summary>
    public string SeedValue
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    } = string.Empty;

    /// <summary>
    /// Gets or sets the currently selected rotor count.
    /// </summary>
    public int SelectedRotorCount
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                ShowVisibleRotors();
                OnPropertyChanged();
            }
        }
    } = 3;

    /// <summary>
    /// Gets or sets a value indicating whether or not the Enigma machine configuration should be
    /// embedded into the encrypted text file.
    /// </summary>
    public bool UseEmbeddedConfiguration
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether or not the Accept command can be executed.
    /// </summary>
    /// <remarks>
    /// The only requirement that is checked is that the seed value length is within the valid
    /// range. Everything else is guaranteed to be valid due to the view model design.
    /// </remarks>
    private bool CanAccept => !string.IsNullOrWhiteSpace(SeedValue)
        && (SeedValue.Length is >= MinStringLength and <= MaxSeedLength);

    /// <summary>
    /// Gets a value indicating whether or not the Clear command can be executed.
    /// </summary>
    /// <remarks>
    /// The only requirement is that the seed value text is not null or empty and the seed isn't
    /// auto-generated.
    /// </remarks>
    private bool CanClear => !string.IsNullOrWhiteSpace(SeedValue) && !IsAutoSeedSelected;

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
    /// Clear the contents of the seed value text box.
    /// </summary>
    private void Clear() => SeedValue = string.Empty;

    /// <summary>
    /// Generate random index values for the reflector and rotors.
    /// </summary>
    private void GenerateRandomIndexValues()
    {
        int upperLimit = MaxValue + 1;
        ReflectorIndex = _numberGenerator.GetNext(MinValue, upperLimit);
        RotorIndex1 = _numberGenerator.GetNext(MinValue, upperLimit);
        RotorIndex2 = _numberGenerator.GetNext(MinValue, upperLimit);
        RotorIndex3 = _numberGenerator.GetNext(MinValue, upperLimit);
        RotorIndex4 = _numberGenerator.GetNext(MinValue, upperLimit);
        RotorIndex5 = _numberGenerator.GetNext(MinValue, upperLimit);
        RotorIndex6 = _numberGenerator.GetNext(MinValue, upperLimit);
        RotorIndex7 = _numberGenerator.GetNext(MinValue, upperLimit);
        RotorIndex8 = _numberGenerator.GetNext(MinValue, upperLimit);
    }

    /// <summary>
    /// Pick a random rotor count within the valid range.
    /// </summary>
    private void GenerateRandomRotorCount()
        => SelectedRotorCount = _numberGenerator.GetNext(MinRotorCount, MaxRotorCount + 1);

    /// <summary>
    /// Generate a random seed string value having a random length between 32 and 64.
    /// </summary>
    private void GenerateRandomSeedValue()
    {
        int seedLength = _numberGenerator.GetNext(AutoSeedMinLength, MaxSeedLength);
        char[] seedChars = new char[seedLength];

        for (int i = 0; i < seedLength; i++)
        {
            seedChars[i] = (char)(_numberGenerator.GetNext(MinValue, MaxValue) + MinChar);
        }

        SeedValue = new string(seedChars);
    }

    /// <summary>
    /// Show or hide rotor index value selectors based on the selected rotor count.
    /// </summary>
    private void ShowVisibleRotors()
    {
        IsRotor4Visible = SelectedRotorCount > 3;
        IsRotor5Visible = SelectedRotorCount > 4;
        IsRotor6Visible = SelectedRotorCount > 5;
        IsRotor7Visible = SelectedRotorCount > 6;
        IsRotor8Visible = SelectedRotorCount > 7;
    }
}