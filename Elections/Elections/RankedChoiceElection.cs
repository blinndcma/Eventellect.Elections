using Elections.Interfaces;

namespace Elections.Elections;

public class RankedChoiceElection : IElection<IRankedBallot>
{
    public ICandidate Run(IReadOnlyList<IRankedBallot> ballots, IReadOnlyList<ICandidate> candidates)
    {
        if (ballots == null || !ballots.Any())
            throw new ArgumentNullException(nameof(ballots));

        var eliminatedCandidates = new List<ICandidate>();
        
        for (var i = 0; i < candidates.Count - 1; i++)
        {
            var candidateVoteCount = new Dictionary<ICandidate, int>();

            foreach (var ballot in ballots)
            {
                foreach (var rankedVote in ballot.Votes.OrderBy(v => v.Rank))
                {
                    var candidateVote = rankedVote.Candidate;

                    //invalid candidate, skip
                    if (!candidates.Contains(candidateVote))
                        continue;

                    if (!eliminatedCandidates.Contains(candidateVote))
                    {
                        if (candidateVoteCount.ContainsKey(candidateVote))
                        {
                            candidateVoteCount[candidateVote]++;
                        }
                        else
                        {
                            candidateVoteCount.Add(candidateVote, 1);
                        }

                        break;
                    }
                }
            }

            var maxCount = candidateVoteCount.Values.Max();
            var totalValidBallots = candidateVoteCount.Values.Sum();
            var percentage = (double) maxCount / totalValidBallots;

            //if not majority winner, eliminate lowest candidate(s)
            if (percentage < 0.5)
            {
                var lowestCount = candidateVoteCount.Values.Min();
                var lowestCandidate = candidateVoteCount.Where(c => c.Value == lowestCount).ToList();

                eliminatedCandidates.AddRange(lowestCandidate.Select(c => c.Key));
            }
            else
            {
                var winningCandidate = candidateVoteCount.Where(c => c.Value == maxCount).ToList(); 

                if (winningCandidate.Count == 1)
                {
                    return winningCandidate.First().Key;
                }
                else if (winningCandidate.Count > 1)
                {
                    throw new Exception("There is a tie.");
                }
                else if (winningCandidate.Count <= 0)
                {
                    throw new Exception("Something bad happened.");
                }
            }
        }

        return null;
    }
}
