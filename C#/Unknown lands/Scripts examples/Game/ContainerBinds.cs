using Assets.Project.Scripts.Opponent;
using Assets.Project.Scripts.Test;
using Assets.Scripts;
using Assets.Scripts.Player;
using UnityEngine;
using Zenject;


namespace Assets.Project.Scripts.Game {
    public class ContainerBinds : MonoInstaller<ContainerBinds> {
        public GameObject spearPrefab;
        public override void InstallBindings() {
            Container.Bind<OpponentStatsNames>()
                .FromInstance(new OpponentStatsNames())
                .AsSingle()
                .NonLazy();

            Container.Bind<PlayerData>()
                .FromInstance(GetComponent<PlayerDataController>().Initialize())
                .AsSingle()
                .NonLazy();


            /*
             * Не работает почему-то, создаётся лишь один экземпляр класса
            Container.Bind<AbstractStateController>()
                .To<StateController>()
                .FromInstance(new StateController())
                .AsTransient()
                .NonLazy();
            */

            Container.BindInterfacesTo<AbstractBulletManager>().AsSingle();
            Container.BindFactory<AbstractBullet, AbstractBullet.Factory>().FromComponentInNewPrefab(spearPrefab);

            //ScribtableObjectContainer.Install(Container);
            /*
            Container.Bind<IMovementVehicle>()
                .To<Legs>()
                .AsSingle()
                .NonLazy();

            Container.Bind<Rigidbody2D>()
                .FromInstance(GetComponent<Rigidbody2D>())
                .AsSingle()
                .NonLazy();
            */
            //Container.Bind<string>().WithId("msg").FromInstance(text).AsTransient();
            //Container.Bind<float>().WithId("speed").FromInstance(speed).AsTransient();
            //Container.Bind<DI>().AsSingle();
            //Container.Bind<DI>().FromInstance<...>();
            //Container.Bind<DI>().FromFactory<...>();
            //Container.Bind<DI>().AsSingle().WithArguments(...).NonLazy();
        }

    }
}
