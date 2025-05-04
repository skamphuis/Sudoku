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
            MaxValue = maxValue;

            includeAll();
        }

        private void includeAll()
        {
            PossibleValues.Clear();
            for (int i = 1; i <= MaxValue; i++)
            {
                PossibleValues.Insert(i - 1, i);
            }

            ExcludedValues = [];
        }

        /// <summary>
        /// Square number in the board, 0-based, right to left, top to bottom.
        /// </summary>
        public int Number { get; set; }
        public int MaxValue { get; }
        public int BoxSize => (int)Math.Sqrt(MaxValue);
        public int RowIndex => (int)Math.Floor((double)(Number - 1) / MaxValue);
        public int ColumnIndex => (Number - 1) % MaxValue;
        /// <summary>
        /// Column index of the box this square is in
        /// </summary>
        public int BoxColumnIndex => (int)Math.Floor((decimal)(ColumnIndex / BoxSize));
        /// <summary>
        /// Row index of the box this square is in
        /// </summary>
        public int BoxRowIndex => (int)Math.Floor((decimal)(RowIndex / BoxSize));
        /// <summary>
        /// In which column within the box is this square? 
        /// </summary>
        public int BoxColumnPos => ColumnIndex - (BoxColumnIndex * BoxSize);
        /// <summary>
        /// In which row within the box is this square? 
        /// </summary>
        public int BoxRowPos => RowIndex - (BoxRowIndex * BoxSize);

        public List<int> ExcludedValues { get; private set; } = [];
        public List<int> PossibleValues { get; } = [];

        private int _presetValue = -1;
        public int PresetValue
        {
            get => _presetValue;
            set
            {
                if (IsKnown)
                {

                }
                //value must be in PossibleValues
                if (!PossibleValues.Contains(value))
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

                OnSquareSolved(this, new SquareSolvedEventArgs(this)
                {
                    IsPreset = true,
                    KnownValue = _presetValue
                });
            }
        }

        private int _solvedValue = -1;
        public int SolvedValue
        {
            get => _solvedValue;
            set
            {
                //value must be in PossibleValues
                if (!PossibleValues.Contains(value))
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

                    OnSquareSolved(this, new SquareSolvedEventArgs(this)
                    {
                        IsPreset = false,
                        KnownValue = _solvedValue
                    });
                }
            }
        }

        public bool IsKnown => (IsPreset || IsSolved);
        public bool IsPreset => (_presetValue > 0);
        public bool IsSolved => ((IsPreset == false) && (PossibleValues.Count == 1 || _solvedValue>0));
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
                else if (PossibleValues.Count == 1)
                {
                    return PossibleValues[0];
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
            if (!PossibleValues.Contains(valueToInclude))
            {
                PossibleValues.Add(valueToInclude);
            }

            //Possibly remove from PossibleValues
            if (ExcludedValues.Contains(valueToInclude))
            {
                ExcludedValues.Remove(valueToInclude);
            }

            OnSquareExcludedChanged(this, new ExcludedChangedEventArgs(valueToInclude, false));
        }

        public void ExcludeValue(int valueToExclude)
        {
            //first remember if it's a known square
            bool isKnown = IsKnown;

            //see if the value was still an option for this square
            if (PossibleValues.Contains(valueToExclude))
            {
                //if so...remove it
                PossibleValues.Remove(valueToExclude);
                //and add to Excludedvalues
                ExcludedValues.Add(valueToExclude);

                //in case this square didn't already have a value
                //we'll find out if it's known now...
                if (!isKnown)
                {
                    switch (PossibleValues.Count)
                    {
                        case 0:
                            OnSquareNotSolvable(this, EventArgs.Empty);
                            break;
                        case 1:
                            OnSquareSolved(this, new SquareSolvedEventArgs(this)
                            {
                                IsPreset = false,
                                KnownValue = PossibleValues[0]
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

        public delegate void SquareNotSolvableHandler(object sender, EventArgs e);
        public event SquareNotSolvableHandler OnSquareNotSolvable;

        public delegate void SquareExcludedChangedHandler(object sender, ExcludedChangedEventArgs e);
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

        public int ChangedValue { get; set; }

        public bool IsExcluded { get; set; }
    }
    public class SquareSolvedEventArgs(Square sq) : EventArgs
    {
        public Square Square { get; set; } = sq;
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

        public int PresetValue { get; set; }

        private Square _square;
        public Square Square
        {
            get => _square;
            set => _square = value;
        }
        public override string Message => $"{PresetValue} is not a possible value for square {_square.Number}.";
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

        public int KnownValue { get; set; }

        private Square _square;
        public Square Square
        {
            get => _square;
            set => _square = value;
        }
        public override string Message => $"Value of square {_square.Number} is already known as {KnownValue}.";
    }
}
