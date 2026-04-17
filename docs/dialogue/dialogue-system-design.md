# Dialogue System Design

## Goal

为当前 Unity 2D 动作游戏增加一套可复用的分支对话系统，支持：

- NPC 近距离触发交互
- 文本逐节点推进
- 选项分支跳转
- 对话期间播放玩家对话动画
- 对话期间暂停战斗与场景推进

## Current Gap

当前项目只有以下可复用基础：

- `PlayerStatus.talk` 状态已存在
- 菜单系统已有 `Time.timeScale` 暂停示例
- 场景中已有 `Canvas`、`EventSystem`、玩家和战斗系统

当前缺失：

- 对话数据结构
- NPC 触发器
- 对话 UI
- 对话控制器
- 可复用的暂停原因管理

## Locked Decisions

- 对话内容用 `ScriptableObject` 维护
- 首版支持分支选项
- 对话期间采用统一暂停入口冻结场景推进
- 菜单暂停和对话暂停共用同一个暂停控制器
- 对话 UI 运行时挂到现有 `C_UserUI` 下，不手工大改现有 UI 预制体
- 首版提供一个运行时注入的示例 NPC 和示例对话资产，保证功能闭环

## Non-goals

- 不做任务系统联动
- 不做对话存档进度
- 不做语音、打字机音效、立绘切换
- 不做复杂的对话编辑器工具
- 不做条件判断、奖励发放、商店或任务副作用

## Data Model

### DialogueAsset

字段：

- `dialogueId`
- `displayName`
- `startNodeId`
- `nodes`

### DialogueNodeData

字段：

- `nodeId`
- `speakerName`
- `message`
- `nextNodeId`
- `isEndNode`
- `choices`

规则：

- 有选项时优先使用 `choices`
- 无选项时用 `nextNodeId` 串接线性节点
- `isEndNode` 为真时结束对话

### DialogueChoiceData

字段：

- `choiceText`
- `nextNodeId`

## Runtime Flow

1. 玩家进入 NPC 触发范围。
2. `DialogueTrigger` 注册当前可交互目标。
3. 玩家按交互键或点击交互按钮开始对话。
4. `DialogueController` 申请 `Dialogue` 暂停原因。
5. 玩家切换到对话动画状态。
6. `DialogueUIController` 展示当前节点文本和选项。
7. 玩家继续或选择分支。
8. 到达结束节点后关闭 UI，释放暂停，恢复玩家状态。

## Pause Semantics

对话期间冻结范围：

- 敌人 AI
- 刷怪
- 子弹运动
- 玩家移动、跳跃、冲刺、攻击
- 2D 物理推进

实现策略：

- 新增统一暂停控制器，内部维护暂停原因集合
- 只要存在暂停原因，就设置 `Time.timeScale = 0`
- 玩家输入脚本和敌人决策脚本额外判断暂停态，避免在 `timeScale = 0` 时继续写入状态
- 玩家与对话 NPC 的 Animator 切到 `UnscaledTime`，确保对话动画仍可播放

## UI

运行时创建：

- 对话面板
- 说话人文本
- 正文文本
- 继续按钮
- 选项按钮容器
- 交互提示
- 交互按钮

首版 UI 使用项目现有 `Canvas` 与 `TextMeshProUGUI`，样式保持轻量，不引入新设计系统。

## Acceptance Criteria

- 玩家可在场景中接近示例 NPC 并开启对话
- 对话框能展示多节点文本
- 能展示至少一个分支选择
- 选择后能跳转到不同节点
- 对话期间敌人和刷怪停止推进
- 对话期间玩家不能移动、冲刺、跳跃和攻击
- 对话期间玩家能播放对话动画
- 对话结束后战斗与玩家控制恢复
