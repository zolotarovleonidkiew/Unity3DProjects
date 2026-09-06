/// <summary>
/// Basic hero data class
/// </summary>
public abstract class HeroData
{
    public int Health;
    public float Speed;
    public float DetectionRange;
    public float AttackCooldown = 1f;
}

public class HeroActivist : HeroData
{
    public HeroActivist()
    {
        Health = 8;
        Speed = 5f;
        DetectionRange = 10f;
        AttackCooldown = 1f;
    }
}