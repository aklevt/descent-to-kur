using System.Collections;
using System.Linq;
using UnityEngine;

namespace Entities
{
    /// <summary>
    /// Базовый класс для всех врагов
    /// Конкретные враги переопределяют ExecuteAction для своей логики
    /// </summary>
    public abstract class EnemyBase : BaseEntity
    {
        protected override void Start()
        {
            base.Start();
            InitializeOnGrid();
    
            if (TurnManager.Instance != null)
            {
                TurnManager.Instance.RegisterEnemy(this);
            }
        }

        protected virtual void OnDestroy()
        {
            if (TurnManager.Instance != null)
                TurnManager.Instance.UnregisterEnemy(this);
        }

        /// <summary>
        /// Выполнить полный ход врага: движение и действие
        /// </summary>
        public IEnumerator DoTurn()
        {
            var health = GetComponent<Health>();
            if (health == null || health.IsDead) yield break;
        
            var canAct = OnTurnStart();
    
            if (!canAct)
            {
                yield return new WaitForSeconds(GetScaledTime(0.5f));
                yield break;
            }

        
            Debug.Log($"{gameObject.name} готовится к ходу...");
            yield return new WaitForSeconds(GetScaledTime(0.1f));

            var bestMove = GetBestMove();

            if (bestMove.HasValue && bestMove.Value != CurrentCell)
            {
                MoveToCell(bestMove.Value);

                while (IsMoving)
                {
                    yield return null;
                }
            }

            if (health.IsDead) yield break;

            yield return new WaitForSeconds(GetScaledTime(0.1f));

            yield return ExecuteAction();

            yield return new WaitForSeconds(GetScaledTime(0.1f));
            Debug.Log($"{gameObject.name} закончил ход");
        }

        /// <summary>
        /// Вычисляет лучший ход в сторону игрока
        /// </summary>
        protected virtual Vector3Int? GetBestMove()
        {
            if (PlayerMovement.Instance == null) return null;

            var playerCell = PlayerMovement.Instance.CurrentCell;
            var currentDistance = ManhattanDistance(CurrentCell, playerCell);

            var tooClose = Stats.MinimumRange > 0 && currentDistance < Stats.MinimumRange;

            if (!tooClose && currentDistance <= Stats.PreferredAttackRange)
            {
                Debug.Log($"[{gameObject.name}] Уже на дистанции атаки ({currentDistance}/{Stats.PreferredAttackRange})");
                return CurrentCell;
            }

            var possibleMoves = GridManager.Instance.GetWalkableTilesInRange(
                CurrentCell, Stats.MoveRange, gameObject
            );

            if (possibleMoves.Count == 0)
            {
                Debug.LogWarning($"[{gameObject.name}] Нет доступных ходов!");
                return CurrentCell;
            }

            if (tooClose)
            {
                Debug.Log($"[{gameObject.name}] Слишком близко ({currentDistance} < {Stats.MinimumRange}), отступаю");
                
                var retreatMoves = possibleMoves
                    .Where(pos => ManhattanDistance(pos, playerCell) >= Stats.MinimumRange)
                    .ToList();

                if (retreatMoves.Count > 0)
                {
                    var bestRetreat = retreatMoves
                        .OrderBy(pos => Mathf.Abs(ManhattanDistance(pos, playerCell) - Stats.PreferredAttackRange))
                        .ThenByDescending(pos => GetDirectionPriority(pos, playerCell, retreat: true))
                        .First();
                    
                    Debug.Log($"[{gameObject.name}] Отступаю на {bestRetreat}");
                    return bestRetreat;
                }
                else
                {
                    var fallbackRetreat = possibleMoves
                        .OrderByDescending(pos => ManhattanDistance(pos, playerCell))
                        .First();
                    
                    Debug.Log($"[{gameObject.name}] Отступать некуда, иду подальше на {fallbackRetreat}");
                    return fallbackRetreat;
                }
            }

            var bestMove = possibleMoves
                .OrderBy(pos => Mathf.Abs(ManhattanDistance(pos, playerCell) - Stats.PreferredAttackRange))
                .ThenBy(pos => ManhattanDistance(pos, playerCell))
                .ThenByDescending(pos => GetDirectionPriority(pos, playerCell, retreat: false))
                .First();

            var newDistance = ManhattanDistance(bestMove, playerCell);
            Debug.Log($"[{gameObject.name}] Иду: {CurrentCell} → {bestMove} (дистанция: {currentDistance} → {newDistance}, цель: {Stats.PreferredAttackRange})");
            
            return bestMove;
        }

        /// <summary>
        /// Возвращает приоритет направления движения
        /// </summary>
        private float GetDirectionPriority(Vector3Int candidatePos, Vector3Int target, bool retreat)
        {
            var direction = candidatePos - CurrentCell;
            var toTarget = retreat ? (CurrentCell - target) : (target - CurrentCell);
            
            var directionNorm = new Vector3(Mathf.Sign(direction.x), Mathf.Sign(direction.y), 0f);
            var targetNorm = new Vector3(Mathf.Sign(toTarget.x), Mathf.Sign(toTarget.y), 0f);
            
            return Vector3.Dot(directionNorm, targetNorm);
        }

        private int ManhattanDistance(Vector3Int a, Vector3Int b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }

        /// <summary>
        /// Попытается использовать способность по индексу, если:
        /// 1. Индекс верный 
        /// 2. Способность можно использовать
        /// 3. Есть доступная цель
        /// </summary>
        protected IEnumerator TryUseAbility(int abilityIndex)
        {
            if (abilityIndex >= Abilities.Count) yield break;

            var ability = Abilities[abilityIndex];
            if (!ability.CanUse(this)) yield break;

            var target = ability.ChooseTarget(this);
            if (!target.HasValue) yield break;

            yield return StartCoroutine(ability.Execute(this, target.Value));
        }

        /// <summary>
        /// Реализуют конкретные враги для своего хода
        /// </summary>
        protected abstract IEnumerator ExecuteAction();
    }
}