using UnityEngine;
using System.Collections;



namespace Inventory
{

    public interface IItem
    {
        int itemId { get; }


        void SetActive(bool active);
    }



    public interface IFirearm : IItem
    {
        int damage { get; set; }
        float range { get; set; }
        int firerate { get; set; }
        float reloadSpeed { get; set; }
        float accuracy { get; set; }
        int capacity { get; set; }


        void FireProjectile();

        bool Reload();
    }


    public interface IOneHandFirearmc : IFirearm
    {
        int holster { get; }
    }


    public interface ITwoHandFirearm : IFirearm
    {


    }
}
