using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UTalk.Data
{
    public class ActorCollection : List<Actor>
    {
        public void Init(ConversationDatabase database)
        {
            this.Clear();
            this.AddRange(database.Actors);
        }
    }
}
