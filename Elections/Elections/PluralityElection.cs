using Elections.Interfaces;

namespace Elections.Elections;

public class PluralityElection : IElection<ISingleVoteBallot>
{
    public ICandidate Run(IReadOnlyList<ISingleVoteBallot> ballots, IReadOnlyList<ICandidate> candidates)
    {
        if (ballots == null || !ballots.Any())
            throw new ArgumentNullException(nameof(ballots));

        if (candidates == null || !candidates.Any())
            throw new ArgumentNullException(nameof(candidates));

        var candidateVoteCount = new Dictionary<ICandidate, int>();

        foreach (var ballot in ballots)
        {
            var candidateVote = ballot.Vote.Candidate;

            //invalid candidate, skip
            if (!candidates.Contains(candidateVote))
                continue;

            if (candidateVoteCount.ContainsKey(candidateVote))
            {
                candidateVoteCount[candidateVote]++;
            }
            else
            {
                candidateVoteCount.Add(candidateVote, 1);
            }
        }

        var maxCount = candidateVoteCount.Values.Max();
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

        return null;
    }
}
