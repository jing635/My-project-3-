// ============ 消息号 ============

using System.Collections.Generic;

public static class MsgId
{
    public const int LoginReq = 101;   // 客户端 → 服务端：登录
    public const int LoginRes = 102;   // 服务端 → 客户端：登录结果
    public const int RegisterReq = 103;   // 客户端 → 服务端：注册
    public const int RegisterRes = 104;   // 服务端 → 客户端：注册结果
    public const int KickNotify = 105;   // 服务端 → 客户端：你被顶号了
    public const int SaveSettingReq = 201;   // 客户端 → 服务端：保存设置
    public const int SaveSettingRes = 202;   // 服务端 → 客户端：保存结果
    public const int MatchReq = 210;              // 客户端 → 大厅：开始匹配
    public const int MatchRes = 211;              // 大厅 → 客户端：匹配状态
    public const int MatchCancelReq = 212;        // 客户端 → 大厅：取消匹配
    public const int MatchSuccessNotify = 213;    // 大厅 → 客户端：匹配成功，去战斗服
    public const int JoinBattleReq = 301;         // 客户端 → 战斗服：进房间
    public const int JoinBattleRes = 302;         // 战斗服 → 客户端：进房结果
    public const int BattleStartNotify = 303;     // 战斗服 → 客户端：开局
    public const int EnemyLeftNotify = 304;       // 战斗服 → 客户端：对手跑了

}
public static class MatchCode
{
    public const int Queued = 0;       // 已进入队列
    public const int Cancelled = 1;    // 已取消
    public const int Fail = 2;         // 失败
}

// ============ 结果码 ============
public static class Code
{
    public const int Ok = 0;   // 成功
    public const int UserNotFound = 1;   // 用户不存在
    public const int WrongPassword = 2;   // 密码错误
    public const int UserExists = 3;   // 用户名已被占用
}

// ============ 消息基类 ============
public abstract class BaseMsg
{
    public abstract int Id();
    public abstract byte[] Write();
    public abstract void Read(byte[] body);
}

// ============ 登录 ============
public class LoginReq : BaseMsg
{
    public string username = "";
    public string password = "";

    public override int Id() { return MsgId.LoginReq; }

    public override byte[] Write()
    {
        ByteWriter w = new ByteWriter();
        w.WriteString(username);
        w.WriteString(password);
        return w.ToArray();
    }

    public override void Read(byte[] body)
    {
        ByteReader r = new ByteReader(body);
        username = r.ReadString();
        password = r.ReadString();
    }
}

public class LoginRes : BaseMsg
{
    public int code;
    public int userId;
    public string message = "";
    public bool isPlayMusic;
    public bool isPlaySound;
    public float music;
    public float sound;
    public List<CharacterData> characterdatas = new List<CharacterData>();

    public override int Id() { return MsgId.LoginRes; }

    public override byte[] Write()
    {
        ByteWriter w = new ByteWriter();
        w.WriteInt(code);
        w.WriteInt(userId);
        w.WriteString(message);
        w.WriteInt(isPlayMusic ? 1 : 0);
        w.WriteInt(isPlaySound ? 1 : 0);
        w.WriteFloat(music);
        w.WriteFloat(sound);
        w.WriteCharacters(characterdatas);
        return w.ToArray();
    }

    public override void Read(byte[] body)
    {
        ByteReader r = new ByteReader(body);
        code = r.ReadInt();
        userId = r.ReadInt();
        message = r.ReadString();
        isPlayMusic = r.ReadInt() == 1;
        isPlaySound = r.ReadInt() == 1;
        music = r.ReadFloat();
        sound = r.ReadFloat();
        characterdatas = r.ReadCharacters();
    }
}

// ============ 注册 ============
public class RegisterReq : BaseMsg
{
    public string username = "";
    public string password = "";

    public override int Id() { return MsgId.RegisterReq; }

    public override byte[] Write()
    {
        ByteWriter w = new ByteWriter();
        w.WriteString(username);
        w.WriteString(password);
        return w.ToArray();
    }

    public override void Read(byte[] body)
    {
        ByteReader r = new ByteReader(body);
        username = r.ReadString();
        password = r.ReadString();
    }
}

public class RegisterRes : BaseMsg
{
    public int code;
    public string message = "";

    public override int Id() { return MsgId.RegisterRes; }

    public override byte[] Write()
    {
        ByteWriter w = new ByteWriter();
        w.WriteInt(code);
        w.WriteString(message);
        return w.ToArray();
    }

    public override void Read(byte[] body)
    {
        ByteReader r = new ByteReader(body);
        code = r.ReadInt();
        message = r.ReadString();
    }
}

// ============ 顶号通知 ============
public class KickNotify : BaseMsg
{
    public string reason = "";

    public override int Id() { return MsgId.KickNotify; }

    public override byte[] Write()
    {
        ByteWriter w = new ByteWriter();
        w.WriteString(reason);
        return w.ToArray();
    }

    public override void Read(byte[] body)
    {
        ByteReader r = new ByteReader(body);
        reason = r.ReadString();
    }
}

// ============ 保存设置 ============
public class SaveSettingReq : BaseMsg
{
    public bool isPlayMusic;
    public bool isPlaySound;
    public float music;
    public float sound;

    public override int Id() { return MsgId.SaveSettingReq; }

    public override byte[] Write()
    {
        ByteWriter w = new ByteWriter();
        w.WriteInt(isPlayMusic ? 1 : 0);
        w.WriteInt(isPlaySound ? 1 : 0);
        w.WriteFloat(music);
        w.WriteFloat(sound);
        return w.ToArray();
    }

    public override void Read(byte[] body)
    {
        ByteReader r = new ByteReader(body);
        isPlayMusic = r.ReadInt() != 0;
        isPlaySound = r.ReadInt() != 0;
        music = r.ReadFloat();
        sound = r.ReadFloat();
    }
}

public class SaveSettingRes : BaseMsg
{
    public int code;

    public override int Id() { return MsgId.SaveSettingRes; }

    public override byte[] Write()
    {
        ByteWriter w = new ByteWriter();
        w.WriteInt(code);
        return w.ToArray();
    }

    public override void Read(byte[] body)
    {
        code = new ByteReader(body).ReadInt();
    }
}

// ============ 角色数据（协议层，客户端和服务端共用） ============
public class CharacterData
{
    public int CharacterId;
    public string CharacterName = "";
    public string CharacterImage = "";
    public string Skill1 = "";
    public string Skill2 = "";
    public string Skill3 = "";
}

// ============ 匹配 ============
public class MatchReq : BaseMsg
{
    public int characterId;

    public override int Id() { return MsgId.MatchReq; }

    public override byte[] Write()
    {
        ByteWriter w = new ByteWriter();
        w.WriteInt(characterId);
        return w.ToArray();
    }

    public override void Read(byte[] body)
    {
        characterId = new ByteReader(body).ReadInt();
    }
}

public class MatchRes : BaseMsg
{
    public int code;
    public string message = "";

    public override int Id() { return MsgId.MatchRes; }

    public override byte[] Write()
    {
        ByteWriter w = new ByteWriter();
        w.WriteInt(code);
        w.WriteString(message);
        return w.ToArray();
    }

    public override void Read(byte[] body)
    {
        ByteReader r = new ByteReader(body);
        code = r.ReadInt();
        message = r.ReadString();
    }
}

public class MatchCancelReq : BaseMsg
{
    public override int Id() { return MsgId.MatchCancelReq; }
    public override byte[] Write() { return new ByteWriter().ToArray(); }
    public override void Read(byte[] body) { }
}

public class MatchSuccessNotify : BaseMsg
{
    public string ip = "";
    public int port;
    public int roomId;
    public int myCharacterId;
    public int enemyCharacterId;
    public string enemyName = "";

    public override int Id() { return MsgId.MatchSuccessNotify; }

    public override byte[] Write()
    {
        ByteWriter w = new ByteWriter();
        w.WriteString(ip);
        w.WriteInt(port);
        w.WriteInt(roomId);
        w.WriteInt(myCharacterId);
        w.WriteInt(enemyCharacterId);
        w.WriteString(enemyName);
        return w.ToArray();
    }

    public override void Read(byte[] body)
    {
        ByteReader r = new ByteReader(body);
        ip = r.ReadString();
        port = r.ReadInt();
        roomId = r.ReadInt();
        myCharacterId = r.ReadInt();
        enemyCharacterId = r.ReadInt();
        enemyName = r.ReadString();
    }
}

// ============ 战斗服 ============
public class JoinBattleReq : BaseMsg
{
    public int userId;
    public int roomId;
    public int characterId;

    public override int Id() { return MsgId.JoinBattleReq; }

    public override byte[] Write()
    {
        ByteWriter w = new ByteWriter();
        w.WriteInt(userId);
        w.WriteInt(roomId);
        w.WriteInt(characterId);
        return w.ToArray();
    }

    public override void Read(byte[] body)
    {
        ByteReader r = new ByteReader(body);
        userId = r.ReadInt();
        roomId = r.ReadInt();
        characterId = r.ReadInt();
    }
}

public class JoinBattleRes : BaseMsg
{
    public int code;
    public string message = "";

    public override int Id() { return MsgId.JoinBattleRes; }

    public override byte[] Write()
    {
        ByteWriter w = new ByteWriter();
        w.WriteInt(code);
        w.WriteString(message);
        return w.ToArray();
    }

    public override void Read(byte[] body)
    {
        ByteReader r = new ByteReader(body);
        code = r.ReadInt();
        message = r.ReadString();
    }
}

public class BattleStartNotify : BaseMsg
{
    public int roomId;
    public int playerIndex;        // 0 或 1，帧同步要用
    public int myCharacterId;
    public int enemyCharacterId;
    public int randomSeed;         // 战斗随机种子，两边必须一样

    public override int Id() { return MsgId.BattleStartNotify; }

    public override byte[] Write()
    {
        ByteWriter w = new ByteWriter();
        w.WriteInt(roomId);
        w.WriteInt(playerIndex);
        w.WriteInt(myCharacterId);
        w.WriteInt(enemyCharacterId);
        w.WriteInt(randomSeed);
        return w.ToArray();
    }

    public override void Read(byte[] body)
    {
        ByteReader r = new ByteReader(body);
        roomId = r.ReadInt();
        playerIndex = r.ReadInt();
        myCharacterId = r.ReadInt();
        enemyCharacterId = r.ReadInt();
        randomSeed = r.ReadInt();
    }
}

public class EnemyLeftNotify : BaseMsg
{
    public string reason = "";

    public override int Id() { return MsgId.EnemyLeftNotify; }

    public override byte[] Write()
    {
        ByteWriter w = new ByteWriter();
        w.WriteString(reason);
        return w.ToArray();
    }

    public override void Read(byte[] body)
    {
        reason = new ByteReader(body).ReadString();
    }
}
