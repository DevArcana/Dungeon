namespace EntityLogic.AI
{
    public struct Influence
    {
        public float playerInfluence;
        public float enemyInfluence;
        public float overallInfluence;

        public Influence(float playerInfluence, float enemyInfluence, float overallInfluence)
        {
            this.playerInfluence = playerInfluence;
            this.enemyInfluence = enemyInfluence;
            this.overallInfluence = overallInfluence;
        }
    }
}