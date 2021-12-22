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
        }
    }
}

