using System;
using System.Collections.Generic;

[Serializable]
public class DialogueData
{
    [Serializable]
    public class DialogueLine
    {
        public string speaker;       // 说话者名字
        public string content;      // 对话内容
        public float typingSpeed;   // 打字速度
        public bool showPlayer;     // 是否显示主角
        public bool showNPC;        // 是否显示NPC
        public bool fadeInPlayer;   // 是否需要淡入主角
        public bool fadeInNPC;      // 是否需要淡入NPC
    }

    public List<DialogueLine> lines = new List<DialogueLine>();
}