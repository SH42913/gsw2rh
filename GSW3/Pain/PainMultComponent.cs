namespace GSW3.Pain
{
    public class PainMultComponent
    {
        public float Multiplier;

        public override string ToString()
        {
            return nameof(PainMultComponent) + ": " +
                   nameof(Multiplier) + " " + Multiplier + "; ";
        }
    }
}