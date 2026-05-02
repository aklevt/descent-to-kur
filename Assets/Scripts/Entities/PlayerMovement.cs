namespace Entities
{
    public class PlayerMovement : BaseEntity
    {
        public static PlayerMovement Instance { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        protected override void Start()
        {
            base.Start();
            InitializeOnGrid();
        }
    }
}