using System;
using System.ComponentModel;
using System.Timers;
using BrewMaster.Messaging;
using Xamarin.Forms;

namespace BrewMaster.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public static MainPageViewModel Current;

        //This lets keeps track of if we have
        //performed actions upon brewing completion
        public bool HasFinishedBrewing { get; set; } = true;

        public MainPageViewModel()
        {
            System.Timers.Timer timer = new System.Timers.Timer( 23 );
            timer.Elapsed += TimerElapsed;
            timer.AutoReset = true;
            timer.Start();

            Current = this;

            this.SendEvent( BrewMasterEventType.NoEvent );

        }

        public void StartFullBrew()
        {
            StartBrew( TimeSpan.FromMinutes( 7 ) );
            this.SendEvent( BrewMasterEventType.BrewStart );
        }

        public void StartHalfBrew()
        {
            StartBrew( TimeSpan.FromMinutes( 5 ) );
            this.SendEvent( BrewMasterEventType.BrewStart );
        }

        private void StartBrew( TimeSpan brewLength )
        {
            HasFinishedBrewing = false;
            BrewStartDateTime = DateTime.Now;
            BrewDateTime = DateTime.Now + brewLength;
        }

        public DateTime BrewStartDateTime { get; set; }


        private void TimerElapsed( Object source, ElapsedEventArgs e )
        {
            var diff = e.SignalTime - BrewDateTime;

            var hasFinishedBrewing = diff >= TimeSpan.Zero;
            if ( hasFinishedBrewing && !HasFinishedBrewing )
            {
                HandleFinishedBrewing();
            }

            SetBrewLocation( diff );
            SetDropLocation( diff );

            if ( diff > TimeSpan.FromHours( 4 ) )
            {
                IsOld = true;
                IsFresh = false;
            }
            else
            {
                IsOld = false;
                IsFresh = true;
            }

            TimeString = diff.ToString();
            Negative = !hasFinishedBrewing ? "-" : string.Empty;

            diff = diff.Duration();

            if ( !hasFinishedBrewing )
            {
                Status = "Brewing";
            }
            else if ( diff < TimeSpan.FromMinutes( 30 ) )
            {
                Status = "FRESH!!";
            }
            else
            {
                Status = "Ready";
            }

            Hours = diff.Hours.ToString();

            var minutes = diff.Minutes.ToString();

            while ( minutes.Length < 2 )
            {
                minutes = "0" + minutes;
            }

            Minutes = minutes;

            var seconds = diff.Seconds.ToString();

            while ( seconds.Length < 2 )
            {
                seconds = "0" + seconds;
            }

            Seconds = seconds;

            var days = Math.Floor( diff.TotalDays );
            if ( days > 0 )
            {
                var dayText = days > 1 ? "days" : "day";
                CoffeeAge = $"{days} {dayText}";
            }
            else
            {
                var hoursText = diff.Hours != 1 ? "hours" : "hour";
                CoffeeAge = $"{Hours} {hoursText}";
            }
        }

        private void HandleFinishedBrewing()
        {
            HasFinishedBrewing = true;
            this.SendEvent( BrewMasterEventType.BrewComplete );
        }

        private double dropLeft = 0;
        private void SetDropLocation( TimeSpan diff )
        {
            if ( diff >= TimeSpan.Zero )
            {
                DropLocation = new Thickness( -100, -100, 0, 0 );
            }

            var t = new Thickness( 0, 0, 0, 0 );
            t.Top = ( 500 - ( diff.Duration().TotalMilliseconds % 500 ) ) - 100;

            if ( t.Top < -50 )
            {
                var random = new Random();
                dropLeft = random.NextDouble() * 1100;
            }
            t.Left = dropLeft;

            DropLocation = t;

        }

        private void SetBrewLocation( TimeSpan diff )
        {
            if ( diff == TimeSpan.Zero )
            {
                BrewLocation = new Thickness();
            }

            var t = new Thickness( 0, 0, 0, 0 );
            var total = ( BrewDateTime - BrewStartDateTime ).TotalMilliseconds;
            var remaining = ( BrewDateTime - DateTime.Now ).TotalMilliseconds;

            if ( total == 0 )
            {
                BrewLocation = t;
            }

            var percent = ( ( double ) remaining ) / total * 100;

            t.Top = percent * 3.3;

            var ms = DateTime.Now.Millisecond;
            var left = ms;
            if ( DateTime.Now.Second % 2 == 0 )
            {
                left = 1000 - left;
            }

            t.Left = -left * 2;
            t.Top = Math.Max( -10, t.Top );
            BrewLocation = t;
        }

        public DateTime BrewDateTime { get; set; } = DateTime.Now;

        private string _timeString = string.Empty;

        public string TimeString
        {
            get => _timeString;
            set
            {
                _timeString = value;
                PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( "TimeString" ) );
            }
        }


        private string _negative = string.Empty;

        public string Negative
        {
            get => _negative;
            set
            {
                _negative = value;
                PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( "Negative" ) );
            }
        }

        private string _hours = string.Empty;

        public string Hours
        {
            get => _hours;
            set
            {
                _hours = value;
                PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( "Hours" ) );
            }
        }

        private string _minutes = string.Empty;

        public string Minutes
        {
            get => _minutes;
            set
            {
                _minutes = value;
                PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( "Minutes" ) );
            }
        }


        private string _seconds = string.Empty;

        public string Seconds
        {
            get => _seconds;
            set
            {
                _seconds = value;
                PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( "Seconds" ) );
            }
        }

        private bool _isFresh = true;

        public bool IsFresh
        {
            get => _isFresh;
            set
            {
                _isFresh = value;
                PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( "IsFresh" ) );
            }
        }

        private bool _isOld = false;

        public bool IsOld
        {
            get => _isOld;
            set
            {
                _isOld = value;
                PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( "IsOld" ) );
            }
        }

        private string _status = string.Empty;

        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( "Status" ) );
            }
        }

        private Thickness _brewLocation = new Thickness();

        public Thickness BrewLocation
        {
            get => _brewLocation;
            set
            {
                _brewLocation = value;
                PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( "BrewLocation" ) );
            }
        }

        private Thickness _dropLocation = new Thickness();

        public Thickness DropLocation
        {
            get => _dropLocation;
            set
            {
                _dropLocation = value;
                PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( "DropLocation" ) );
            }
        }

        private string _coffeeAge = string.Empty;

        public string CoffeeAge
        {
            get => _coffeeAge;
            set
            {
                _coffeeAge = value;
                PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( "CoffeeAge" ) );
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
    }
}
