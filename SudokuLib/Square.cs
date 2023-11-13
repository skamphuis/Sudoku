using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;

namespace SudokuLib
{
    [DefaultProperty("Number")]
    public class Square
    {
        public Square(int squareNumber, int maxValue)
        {
            Number = squareNumber;
            _maxValue = maxValue;

            includeAll();
        }

        private void includeAll()
        {
            _possibleValues.Clear();
            for (int i = 1; i <= MaxValue; i++)
            {
                _possibleValues.Insert(i - 1, i);
            }

            _excludedValues = new List<int>();
        }

        private int _Number;
        public int Number
        {
            get { return _Number; }
            set { _Number = value; }
        }

        private int _maxValue;
        public int MaxValue
        {
            get { return _maxValue; }
        }

        public int RowIndex
        {
            get
            {
                return (int)Math.Floor((double)(Number - 1) / MaxValue);
                //return (int)Math.Floor((double)SquareNumber / MaxValue) + (SquareNumber % MaxValue);
            }
        }

        public int ColumnIndex
        {
            get { return (Number - 1) % MaxValue; }
        }

        private List<int> _excludedValues = new List<int>();
        public List<int> ExcludedValues
        {
            get { return _excludedValues; }
        }

        private List<int> _possibleValues = new List<int>();
        public List<int> PossibleValues
        {
            get { return _possibleValues; }
        }

        //public void UnsetValue()
        //{
        //    //remember the current preset
        //    int oldPreset = _presetValue;

        //    if (_presetValue > 0)
        //    {
        //        _presetValue = -1;
        //        OnSquareUnSolved(this, new SquareSolvedEventArgs(oldPreset, true));
        //    }
        //}

        private int _presetValue = -1;
        public int PresetValue
        {
            get { return _presetValue; }
            set
            {
                if (IsKnown)
                {

                }
                //value must be in PossibleValues
                if (!_possibleValues.Contains(value))
                {
                    throw new InvalidPresetException(this, value);
                }

                //if there was already a presetValue, we need to unsolve the square first
                //UnsetValue();

                //Set the new presetvalue
                _presetValue = value;

                //Leave the possible values as-is, to allow change of presets
                ////Exclude all other values
                //_possibleValues=new List<int>();
                //_possibleValues.Add(_presetValue);

                OnSquareSolved(this, new SquareSolvedEventArgs()
                {
                    IsPreset = true,
                    KnownValue = _presetValue
                });
            }
        }

        private int _solvedValue = -1;
        public int SolvedValue
        {
            get { return _solvedValue; }
            set
            {
                //value must be in PossibleValues
                if (!_possibleValues.Contains(value))
                {
                    throw new InvalidPresetException(this, value);
                }

                //if the square already has a different value: things are in error
                if (Value > 0 && Value != value)
                {
                    OnSquareNotSolvable(this, EventArgs.Empty);
                }
                else
                {
                    //Set the new solvedvalue
                    _solvedValue = value;

                    OnSquareSolved(this, new SquareSolvedEventArgs()
                    {
                        IsPreset = false,
                        KnownValue = _solvedValue
                    });
                }
            }
        }

        //public void ResetSquare()
        //{
        //    includeAll();
        //    OnSquareReset(this, new EventArgs());
        //}

        public bool IsKnown
        {
            get { return (IsPreset || IsSolved); }
        }

        public bool IsPreset
        {
            get { return (_presetValue > 0); }
        }

        public bool IsSolved
        {
            get { return ((IsPreset == false) && (_possibleValues.Count == 1 || _solvedValue>0)); }
        }

        public int Value
        {
            get
            {
                if (PresetValue > 0)
                {
                    return PresetValue;
                }
                else if (_solvedValue > 0)
                {
                    return _solvedValue;
                }
                else if (_possibleValues.Count == 1)
                {
                    return _possibleValues[0];
                }
                else
                {
                    return -1;
                }

            }
        }

        public void IncludeValue(int valueToInclude)
        {
            //Possibly add to PossibleValues
            if (!_possibleValues.Contains(valueToInclude))
            {
                _possibleValues.Add(valueToInclude);
            }

            //Possibly remove from PossibleValues
            if (_excludedValues.Contains(valueToInclude))
            {
                _excludedValues.Remove(valueToInclude);
            }

            OnSquareExcludedChanged(this, new ExcludedChangedEventArgs(valueToInclude, false));
        }
        public void ExcludeValue(int valueToExclude)
        {
            //first remember if it's a known square
            bool isKnown = IsKnown;

            //see if the value was still an option for this square
            if (_possibleValues.Contains(valueToExclude))
            {
                //if so...remove it
                _possibleValues.Remove(valueToExclude);
                //and add to Excludedvalues
                _excludedValues.Add(valueToExclude);

                //in case this square didn't already have a value
                //we'll find out if it's known now...
                if (!isKnown)
                {
                    switch (_possibleValues.Count)
                    {
                        case 0:
                            OnSquareNotSolvable(this, new EventArgs());
                            break;
                        case 1:
                            OnSquareSolved(this, new SquareSolvedEventArgs()
                            {
                                IsPreset = false,
                                KnownValue = _possibleValues[0]
                            });
                            break;
                        default:
                            break;
                    }
                }

                OnSquareExcludedChanged(this, new ExcludedChangedEventArgs(valueToExclude, true));
            }
        }

        public event EventHandler<SquareSolvedEventArgs> OnSquareSolved;

        //public delegate void SquareUnSolvedHandler(Square sender, SquareSolvedEventArgs e);
        //public event SquareUnSolvedHandler OnSquareUnSolved;

        public delegate void SquareNotSolvableHandler(Square sender, EventArgs e);
        public event SquareNotSolvableHandler OnSquareNotSolvable;

        public delegate void SquareExcludedChangedHandler(Square sender, ExcludedChangedEventArgs e);
        public event SquareExcludedChangedHandler OnSquareExcludedChanged;

        //public delegate void SquareResetHandler(Square sender, EventArgs e);
        //public event SquareResetHandler OnSquareReset;
    }
    public class ExcludedChangedEventArgs : EventArgs
    {
        public ExcludedChangedEventArgs()
        {
        }

        public ExcludedChangedEventArgs(int changedValue, bool isExcluded)
        {
            this.ChangedValue = changedValue;
            this.IsExcluded = isExcluded;
        }

        private int _changedValue;
        public int ChangedValue
        {
            get { return _changedValue; }
            set { _changedValue = value; }
        }

        private bool _isExcluded;
        public bool IsExcluded
        {
            get { return _isExcluded; }
            set { _isExcluded = value; }
        }
    }
    public class SquareSolvedEventArgs : EventArgs
    {
        public bool IsPreset { get; set; }
        public int KnownValue { get; set; }
    }

    public class InvalidPresetException : Exception
    {
        public InvalidPresetException()
        {
        }
        public InvalidPresetException(Square sq, int presetValue)
        {
            this.Square = sq;
            this.PresetValue = presetValue;
        }

        private int _presetValue;
        public int PresetValue
        {
            get { return _presetValue; }
            set { _presetValue = value; }
        }

        private Square _square;
        public Square Square
        {
            get { return _square; }
            set { _square = value; }
        }
        public override string Message
        {
            get { return String.Format("{0} is not a possible value for square {1}.", _presetValue, _square.Number); ; }
        }
    }
    public class SquareAlreadyKnownException : Exception
    {
        public SquareAlreadyKnownException()
        {
        }
        public SquareAlreadyKnownException(Square sq, int knownValue)
        {
            this.Square = sq;
            this.KnownValue = knownValue;
        }

        private int _knownValue;
        public int KnownValue
        {
            get { return _knownValue; }
            set { _knownValue = value; }
        }

        private Square _square;
        public Square Square
        {
            get { return _square; }
            set { _square = value; }
        }
        public override string Message
        {
            get { return String.Format("Value of square {0} is already known as {1}.", _square.Number, _knownValue);  }
        }
    }
}
