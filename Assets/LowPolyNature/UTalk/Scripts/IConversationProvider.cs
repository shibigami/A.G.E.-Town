using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTalk
{
    public interface IConversationProvider
    {
        String GetConversationName();
        void SetConversationName(string name);
    }
}
