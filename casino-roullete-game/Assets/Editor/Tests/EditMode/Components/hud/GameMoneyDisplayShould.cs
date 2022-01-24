using System.Collections;
using System.Collections.Generic;
using Components;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using ViewModel;

namespace Editor.Tests.Components.hud
{
    public class GameMoneyDisplayShould
    {
        private GameObject _gameObject;
        private GameObject _gameObjectBet;
        private GameObject _gameObjectMoney;

        private Text _textFieldBet;
        private Text _textFieldMoney;

        [SetUp]
        public void SetUp()
        {
            _gameObject = new GameObject();
            _gameObjectBet = new GameObject();
            _gameObjectMoney = new GameObject();
            
            _textFieldBet = _gameObjectBet.AddComponent<Text>();
            _textFieldMoney = _gameObjectMoney.AddComponent<Text>();
            _textFieldBet.text = "";
        }

        [Test]
        public void show_player_money()
        {
            var component = _gameObject.AddComponent<GameMoneyDisplay>();
            component.betLabel = _textFieldBet;
            component.moneyLabel = _textFieldMoney;
            component.characterTable = ScriptableObject.CreateInstance<CharacterTable>();
            component.characterTable.characterMoney = ScriptableObject.CreateInstance<CharacterMoney>();
            component.Start();

            component.characterTable.characterMoney.characterBet.Value = 20;  
            component.characterTable.characterMoney.characterMoney.Value = 100;  

            Assert.AreEqual("20", _textFieldBet.text);
            Assert.AreEqual("100", _textFieldMoney.text);
        }
    }
}
