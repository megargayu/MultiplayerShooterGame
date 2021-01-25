public interface Gun
{
    float damage { get; }
    float speed { get; }
    bool isMultishot { get; }
    float shootSpeed { get; }
    float weight { get; }
    float reloadSpeed { get; }
    int ammoCapacity { get; }

    int ammoLeft { get; set; }
}

public class StartGun : Gun
{
    public float damage => 10;
    public float speed => 1000;
    public bool isMultishot => false;
    public float shootSpeed => 0.2f;
    public float weight => 1;
    public float reloadSpeed => 1;
    public int ammoCapacity => 5;

    private int _ammoLeft = 5;
    public int ammoLeft { get => _ammoLeft; set => _ammoLeft = value; }
}