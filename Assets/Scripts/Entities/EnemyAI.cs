using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Entities;

namespace Entities
{
    /// <summary>
    /// Отвечает за AI логику врагов: выбор лучшего хода, позиционирование относительно игрока
    /// </summary>
    public class EnemyAI
    {
        /// <summary>
        /// Вычисляет оптимальную клетку для перемещения врага
        /// </summary>
        public Vector3Int? CalculateBestMove(EnemyBase enemy)
        {
            if (PlayerMovement.Instance == null) return null;

            var playerCell = PlayerMovement.Instance.CurrentCell;
            var currentDistance = ManhattanDistance(enemy.CurrentCell, playerCell);
            
            if (!enemy.Stats.IsInDetectionRange(enemy.CurrentCell, playerCell))
            {
                Debug.Log($"[{enemy.gameObject.name}] Игрок вне радиуса обнаружения ({currentDistance} > {enemy.Stats.DetectionRadius})");
                return null;
            }
            
            // Проверка дистанции
            var tooClose = enemy.Stats.MinimumRange > 0 && currentDistance < enemy.Stats.MinimumRange;
            var inAttackRange = !tooClose && currentDistance <= enemy.Stats.PreferredAttackRange;

            if (inAttackRange)
            {
                Debug.Log($"[{enemy.gameObject.name}] Уже на дистанции атаки ({currentDistance}/{enemy.Stats.PreferredAttackRange})");
                return enemy.CurrentCell;
            }

            var possibleMoves = GridManager.Instance.GetWalkableCellsInRange(
                enemy.CurrentCell, enemy.Stats.MoveRange, enemy.gameObject
            );

            if (possibleMoves.Count == 0)
            {
                Debug.LogWarning($"[{enemy.gameObject.name}] Нет доступных ходов!");
                return enemy.CurrentCell;
            }

            return tooClose 
                ? CalculateRetreatMove(enemy, playerCell, possibleMoves, currentDistance)
                : CalculateAdvanceMove(enemy, playerCell, possibleMoves, currentDistance);
        }

        private Vector3Int CalculateRetreatMove(EnemyBase enemy, Vector3Int playerCell, List<Vector3Int> possibleMoves, int currentDistance)
        {
            Debug.Log($"[{enemy.gameObject.name}] Слишком близко ({currentDistance} < {enemy.Stats.MinimumRange}), отступаю");
            
            var retreatMoves = possibleMoves
                .Where(pos => ManhattanDistance(pos, playerCell) >= enemy.Stats.MinimumRange)
                .ToList();

            if (retreatMoves.Count > 0)
            {
                var bestRetreat = retreatMoves
                    .OrderBy(pos => Mathf.Abs(ManhattanDistance(pos, playerCell) - enemy.Stats.PreferredAttackRange))
                    .ThenByDescending(pos => GetDirectionPriority(pos, enemy.CurrentCell, playerCell, retreat: true))
                    .First();
                
                Debug.Log($"[{enemy.gameObject.name}] Отступаю на {bestRetreat}");
                return bestRetreat;
            }
            else
            {
                var fallbackRetreat = possibleMoves
                    .OrderByDescending(pos => ManhattanDistance(pos, playerCell))
                    .First();
                
                Debug.Log($"[{enemy.gameObject.name}] Отступать некуда, иду подальше на {fallbackRetreat}");
                return fallbackRetreat;
            }
        }

        private Vector3Int CalculateAdvanceMove(EnemyBase enemy, Vector3Int playerCell, List<Vector3Int> possibleMoves, int currentDistance)
        {
            var bestMove = possibleMoves
                .OrderBy(pos => Mathf.Abs(ManhattanDistance(pos, playerCell) - enemy.Stats.PreferredAttackRange))
                .ThenBy(pos => ManhattanDistance(pos, playerCell))
                .ThenByDescending(pos => GetDirectionPriority(pos, enemy.CurrentCell, playerCell, retreat: false))
                .First();

            var newDistance = ManhattanDistance(bestMove, playerCell);
            Debug.Log($"[{enemy.gameObject.name}] Иду: {enemy.CurrentCell} → {bestMove} (дистанция: {currentDistance} → {newDistance}, цель: {enemy.Stats.PreferredAttackRange})");
            
            return bestMove;
        }

        private float GetDirectionPriority(Vector3Int candidatePos, Vector3Int currentPos, Vector3Int target, bool retreat)
        {
            var direction = candidatePos - currentPos;
            var toTarget = retreat ? (currentPos - target) : (target - currentPos);
            
            var directionNorm = new Vector3(Mathf.Sign(direction.x), Mathf.Sign(direction.y), 0f);
            var targetNorm = new Vector3(Mathf.Sign(toTarget.x), Mathf.Sign(toTarget.y), 0f);
            
            return Vector3.Dot(directionNorm, targetNorm);
        }

        private int ManhattanDistance(Vector3Int a, Vector3Int b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }
    }
}