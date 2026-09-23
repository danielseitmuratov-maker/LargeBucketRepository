using System;
using System.Collections.Generic;
using _Root._Scripts.Core.GameMap.SpecialZones;
using _Root._Scripts.Infrastructure.Services.Loaders;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Root._Scripts.Core.GameMap
{
    public class GameMapSpecialPointsProvider : IGameMapSpecialPointsProvider, IDisposable
    {
        private readonly ILoader<GameMapRoot> _gameMapLoader;
        private GameMapRoot _gameMapRoot;

        private List<HidingZone> _hidingZones = new List<HidingZone>();
        private List<Transform> _treasureSpawnPoints = new List<Transform>();

        public GameMapSpecialPointsProvider(ILoader<GameMapRoot> gameMapLoader)
        {
            _gameMapLoader = gameMapLoader;
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            _gameMapLoader.Loaded += OnGameMapLoaded;
        }

        private void UnsubscribeFromEvents()
        {
            _gameMapLoader.Loaded -= OnGameMapLoaded;
        }

        public Vector3 GetRandomHidingPoint()
        {
            if (_hidingZones.Count == 0)
                return Vector3.zero;
            
            int randomIndex = Random.Range(0, _hidingZones.Count);
            
            return _hidingZones[randomIndex].GetRandomPointOnNavMesh();
        }

        public Vector3 GetRandomTreaseSpawnPoint()
        {
            if (_treasureSpawnPoints.Count == 0) 
                return Vector3.zero;
            
            int randomIndex = Random.Range(0, _treasureSpawnPoints.Count);
            
            return _treasureSpawnPoints[randomIndex].position;
        }

        public List<Vector3> GetHidingPoints()
        {
            List<Vector3> result = new List<Vector3>();
            
            foreach (var zone in _hidingZones) 
                result.Add(zone.GetRandomPointOnNavMesh());
            
            return result;
        }

        public List<Vector3> GetTreasuresSpawnPoints()
        {
            List<Vector3> result = new List<Vector3>();
            
            foreach (var tp in _treasureSpawnPoints)
                result.Add(tp.position);
            return result;
        }

        private void OnGameMapLoaded(GameMapRoot gameMapRoot, int index)
        {
            _gameMapRoot = gameMapRoot;
            _hidingZones = gameMapRoot.HidingZones;
            _treasureSpawnPoints = gameMapRoot.TreasuresSpawnPoints;
        }

        public void Dispose()
        {
            UnsubscribeFromEvents();
        }
    }
}