using System.Collections;
using System.Collections.Generic;
using Components;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;


namespace Editor.Tests.Scenes
{
    [TestFixture]
    public class GameSceneShould
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/Game.unity");
        }

        [Test]
        public void contain_money_display()
        {
            var component = GameObject.FindObjectOfType<GameMoneyDisplay>();
            Assert.NotNull(component);
            Assert.NotNull(component.betLabel);
            Assert.NotNull(component.moneyLabel);
        }

        [Test]
        public void contain_win_lost_display()
        {
            var component = GameObject.FindObjectOfType<WinLostDisplay>();
            Assert.NotNull(component);
        }

        [Test]
        public void contain_game_reward_display()
        {
            var component_reward = GameObject.FindObjectOfType<GameRewardInput>();
            var component = GameObject.FindObjectOfType<GameRewardDisplay>();
            Assert.NotNull(component_reward);
            Assert.NotNull(component);
        }

        [Test]
        public void contain_last_number_display()
        {
            var component = GameObject.FindObjectOfType<LastTableNumberDisplay>();
            Assert.NotNull(component);
            Assert.NotNull(component.characterTable);
            Assert.NotNull(component.anchorNumbers);
            Assert.NotNull(component.numberContainer);
            Assert.NotNull(component.instanceContainer);
        }
    }
}

