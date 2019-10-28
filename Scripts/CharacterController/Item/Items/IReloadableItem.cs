namespace CharacterController
{

    public interface IReloadableItem
    {
        //string GetReloadInputName();

        bool TryStartReload();

        bool IsReloading();

        //void TryStopReload();


    }

}