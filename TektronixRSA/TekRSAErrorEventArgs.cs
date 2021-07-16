using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Tektronix.TekRSA
{
    public abstract class TekBase : INotifyPropertyChanged
    {
        public event EventHandler<TekRSAErrorEventArgs> Error = delegate { };
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected void OnPropertyChanged([CallerMemberName] string propName="")
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        static Regex regexCamelCase = new Regex("(?<!(^|[A-Z]))(?=[A-Z])|(?<!^)(?=[A-Z][a-z])", RegexOptions.Compiled); 

        protected internal void OnError(string message, [CallerMemberName] string source = "")
        {
            Error.Invoke(this, new TekRSAErrorEventArgs(message, source));
        }

        protected internal void OnError(ReturnStatus message, [CallerMemberName] string source = "")
        {
            var tm = regexCamelCase.Split(message.ToString());
            var mass = String.Join(" ", tm.Select(x => x.ToLower()));

            Error.Invoke(this, new TekRSAErrorEventArgs(mass, source));
        }

    }


    public class TekRSAErrorEventArgs : EventArgs
    {
        public string Source { get; }
        public string Message { get; }

        public TekRSAErrorEventArgs(string message,string source)
        {
            Source = source;
            Message = message;
        }
    }
}
