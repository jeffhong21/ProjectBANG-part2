namespace CharacterController
{

    public enum ActionType
    {
        Idle = 0,
        Movement = 1,
        Airborne = 2
    }

    public static class ActionTypeID
    {

        public static readonly int Idle = 0;
        public static readonly int StartMovement = 2;
        public static readonly int StopMovement = 3;
        public static readonly int QuickTurn = 4;
        public static readonly int Fall = 5;
        public static readonly int Crouch = 6;
        public static readonly int Pickup = 7;
        public static readonly int HitReaction = 8;
        public static readonly int Knockdown = 9;
        public static readonly int GetUp = 10;
        public static readonly int Die = 12;


        public static readonly int Punch = 11;



        public static readonly int Jump = 21;


        public static readonly int Vault = 31;
        public static readonly int Climb1M = 32;

        public static readonly int Roll = 41;
        public static readonly int Slide = 42;

        public static readonly int Cover = 51;
    }



    public static class ItemActionID
    {
        public static readonly int Idle = 0;
        public static readonly int Aim = 1;
        public static readonly int Use = 2;
        public static readonly int Equip = 3;
        public static readonly int Unequip = 4;
        public static readonly int Drop = 5;

        public static readonly int Reload = 10;
        public static readonly int Throw = 15;
    }
}