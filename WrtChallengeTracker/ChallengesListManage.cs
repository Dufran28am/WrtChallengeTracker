using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrtChallengeTracker
{
    static public class ChallengesListManage
    {
        static private List<Challenge> existingChallenges=new List<Challenge>(), filteredChallenge=new List<Challenge>();
        static public List<Challenge> ExistingChallenges { get { return existingChallenges; } }

        static public void AddChallenge(Challenge challenge)=>existingChallenges.Add(challenge);
    
 }
}
