using Prism.Events;
using CodeMovement.EbcdicCompare.Models;

namespace CodeMovement.EbcdicCompare.Presentation.Event
{
    public class WindowResizeEvent : PubSubEvent<WindowSize>
    {
    }
}
