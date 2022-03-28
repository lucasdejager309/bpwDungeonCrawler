public interface IWeapon{
    Range DamageRange {
        get;
        set;
    }

    public int GetDamage();
}