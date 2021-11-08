
using System.Threading.Tasks;

namespace  Managers
{    
    public static class TaskExtension
    {
        public static async void Await(this Task task)
        {
            await task;
        }
    }
}