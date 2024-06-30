using ClassLibrary.Models;

namespace ClassLibrary.BusinessLogic
{
    public interface IMessages
    {

        event EventHandler<MessageEventArgs> MessageSent;
        void SelectMenu();
    }

    public class MessageEventArgs : EventArgs
    {
        public List<Results> Content { get; }

        public MessageEventArgs(List<Results> content)
        {
            Content = content;
        }
    } 
}