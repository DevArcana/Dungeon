namespace EntityLogic.AI
{
    public struct Influence
    {
        public float playersInfluence;
        public float agentsInfluence;
        public float overallInfluence;

        public Influence(float playersInfluence, float agentsInfluence, float overallInfluence)
        {
            this.playersInfluence = playersInfluence;
            this.agentsInfluence = agentsInfluence;
            this.overallInfluence = overallInfluence;
        }
    }
}
