using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrtChallengeTracker.ObjectManaging
{
    static public class ChallengesListManaging
    {
        static private List<Challenge> existingChallenges = new List<Challenge>(), filteredChallenge = new List<Challenge>();
        static public IEnumerable<Challenge> ExistingChallenges { get { return existingChallenges; } }

        static public void AddChallenge(Challenge challenge)
        {
            existingChallenges.Add(challenge);
            filteredChallenge = existingChallenges;
        }

        static public void Remove()
        {
            foreach(Challenge challenge in existingChallenges)
            {
                if(challenge.ToRemove())
                {
                    challenge.BeforeRemoving();
                    existingChallenges.Remove(challenge);
                    
                }
            }

        }

    }
}
