## tolua#
tolua#的优点在此无需过度阐述，本修改版本只集中描述差异化的修改。<br>
源版本传送门：https://github.com/topameng/tolua<br>
本修改版本Plugins库传送门：https://github.com/NewbieGameCoder/tolua_runtime

# 关于Lua"暖更新"C#逻辑
* 对！这次加入了lua函数运行时决定监听C#函数开始执行、监听C#函数return结束、替换整个C#函数实现的特性。 <br>
#### 支持Inject的C#函数类型如下：<br>
 * 构造函数（只支持后置监听。第一自己会调用父类构造，第二替换的情况不多，能善后就足够了)<br>
 * 属性访问器（支持前置监听、后置监听、整体替换、替换并提前调用次父类函数、替换并结束后调用次父类函数。Lua函数替换属性set函数后，需要自己利用反射修改相关类对象的Field）<br>
 * 普通静态、非静态函数（支持前置监听、后置监听、整体替换、替换并提前调用次父类函数、替换并结束后调用次父类函数。底层工具能按照指定格式自动更新C#函数ref、out参数,具体看例子。）<br>
 * 协程(只支持前置监听，后置监听，整体替换。监听的时候，可以监听到协程的状态，比如执行到哪段代码了，不过比较难理解，需要自行摸索。其实一运行demo的example，看到TestCoroutine的打印，应该就能大概明白怎么跟踪协程的状态了。)<br>
#### 不支持Inject的C#函数类型如下：<br>
 * 静态构造函数（技术上支持，被我默认屏蔽。就算Inject后，也不稳定，因为静态构造只调用一次，且调用的时候不一定lua环境初始化好了，且对修bug的作用不是很大）<br>
 * 析构函数（技术上支持，被我默认屏蔽。使用情况较少，且会拉扯C# gc，析构函数应该尽量避免跟业务逻辑打交道）<br>
 * 运算符重载（对修BUG之类的用处不大）<br>
 * 事件的Add、Remove（对修BUG之类的用处不大，反射可以解决大部分需求）<br>
 * unsafe函数（对修BUG之类的用处不大，多用于工具性代码，比如网络库）<br>
 * 匿名函数（我懒，跟Inject协程类似，较复杂）<br>
 * 迭代器IEnumerable函数（我懒，跟Inject协程类似，且对修BUG用处不大，多见于工具性代码）<br>
 * MonoBehaviour的构造函数默认不Inject<br>
 * ScriptableObject的构造函数默认不Inject<br>
 * 泛型函数（已有技术原型实现，多见于工具性代码，且本身特性已经被“重复”验证了多次逻辑的正确性，延后抽空弄泛型展开）<br>
 * 日后补上其他不支持的函数类型<br>
#### 特性：<br>
* **配合提供的以文件夹中的所有cs文件为单位的，黑名单生成工具，以及一些命名空间Drop列表等过滤措施，“半全量”Inject所有C#函数，避免眼睛找瞎。当然代码文件会变大些，我自己的项目3.5M的代码dll文件，Inject后，多了700多KB(Inject了“所有”的构造函数，“所有”的属性访问器函数)，Il2cpp后更多，个人视自己的过滤情况而定O(∩_∩)O~**<br>
* **支持Lua代码运行时决定Inject方式，比如刚开始只用前置监听，就可以在函数执行前回调到lua一次。后面还有需求可以Lua决定是否整个替换C#的函数实现细节，比较灵活多变**<br>
* **支持较精确的重载Inject匹配，提高安全验证，具体可以参考下Examples/TestInjection下的TestOverload在ToLuaInjectionTestInjector.lua.bytes文件中的Inject处理**<br>
* **支持“有限”的增量Injection。如果只是新增C#函数、或者删掉旧C#函数，支持不换包2个客户端运行在线上，且重写C#逻辑的Lua代码运行“基本”正常（新增的C#函数被Inject后，只有新版本的客户端才有效；对于老版本的被删掉的C#函数，只有老版本的C#客户端才会调用到重写删掉的C#函数逻辑的Lua代码。其他情况一切正常）**<br>
* **GC较少**<br>
* **没改tolua runtime，方便升级**<br>

## 本修改目前国内、台湾、英文双平台上线稳定运行
## 本修改出发目的是想简化Inject工作，减少错误发生O(∩_∩)O~。具体使用方式见[Wiki](https://github.com/NewbieGameCoder/tolua/wiki/%E4%BD%BF%E7%94%A8%E8%AF%B4%E6%98%8E)

# 关于Wrap速度的修改
* 用过tolua#的可能经历过，Lua环境初始化的时候，LuaBinder类的Bind花费了大量时间（比如4s），自己需要添加额外的逻辑，分散C#相关模块wrap进lua环境的时机（当然tolua#已经集成了一个PreLoad的概念能解决大部分的问题，某个模块只有在被用到的时候，才会动态整个wrap进lua环境，注意是整个）。而本工程的修改则更进一步，相对于原版用一点点且是一次性的性能消耗，离散C#任意一个模块的函数的wrap时机、变量的wrap时机，使得C#的函数、变量只有在lua端访问的时候才会wrap进lua环境，从而较大的改善某一个时间段整体的wrap速度。目前我自己的工程在编辑器上能获得3.5倍提升，其他工程视项目而定，如果lua环境初始化时候，访问C#各个模块的频率没我自己项目的高，那么会获得更大的提升。<br>
* 本条修改需要集成后重新生成项目的所有wrap文件，可以查看ToLuaExport类里面的enableLazyFeature开启流程，自定义自己的修改。目前是ToLuaMenu里面自动生成代码的时候，自动帮你开了。如果自己修改成完全不用这个特性，那么本修改版本就是原汁原味的ToLua#，我尽量做到不改经过时间验证的整体逻辑，包括runtime库也是新添函数，不改tolua#库已有的逻辑代码。<br>

# 使用lua5.3
* 插件在iOS、Android、windows上默认的lua虚拟机环境为luajit（对应于lua5.1），mac上默认的lua虚拟机环境为lua5.1。<br>
* 目前ToLua也支持全平台统一成lua5.3的lua虚拟机环境。如果要使用lua5.3的版本，步骤如下
* 1、前往https://github.com/topameng/tolua_runtime <br>
* 2、将Plugins53文件夹里面的所有tolua相关的runtime底层库，都拷贝覆盖到unity工程的Plugins目录下。<br>
* 3、打开unity编辑器，添加“**LUAC_5_3**”宏，回车等待编辑器编译完毕既是Lua5.3的虚拟机环境。<br>
* **注意要用lua5.3宏定义“LUAC_5_3”必须一直有效！！如果用luajit或lua5.1版本的，宏定义一定要删除！！！！**<br>
* ToLua已经在ToLuaMenu.cs文件里面集成了自动化的导出32位、64位luajit的bytecode（不能混用），以及32位、64位兼容能混用的lua5.3的bytecode,<br>
* 具体实现请查看ToLuaMenu.cs的改动，现在Lua5.3虚拟机环境的bytecode，默认32位、64位通用。<br>

# 关于arm64下的luajit
* 部分童鞋用最新的arm64的luajit版本库会出现莫名的闪退，32位机子上的armv7完全没问题。经定位，是luajit即时编译某种形式的代码的问题。<br>
* 建议有问题的同学，tolua.lua文件里面加上jit.off(); jit.flush()强制关闭jit模式，开启interpreter模式，并注释jit.opt.start(3)这行相关的代码。 <br>
* 目前ToLua最新版默认在Luajit的版本中，在64位Android平台上，关闭LuaJit的jit模式，无需手动关闭，请周知<br>

# 关于ULong的支持
* ToLua全新集成了Luac 5.3，可以完美支持Long。但是由于ULong的特殊性（负数除法用long代替运算结果可能会出错——“18446744073709552040 / 2”），<br>
* 斟酌再三之后，ToLua的ULong的扩展，依然还是userdata，请周知！！故准备压进lua的ULong数据，能用Long代替就就用Long代替<br>

# 关于Wrap文件bind速度说明
* 在以前IL2cpp没普及的时代，mono下Wrap文件的注册非常耗时，经常有项目耗时达到四五秒，而现在，开启IL2cpp后，这个过程几乎没啥耗时，请周知！！<br>

# 网友学习心得
[Unity3D热更新技术点——ToLua（上）](https://zhuanlan.zhihu.com/p/42472089) <br>
[Unity3D热更新技术点——ToLua（中）](https://zhuanlan.zhihu.com/p/42472115) <br>
[Unity3D热更新技术点——ToLua（下）](https://zhuanlan.zhihu.com/p/43632619) <br>
[tolua学习(一)](https://blog.csdn.net/qq_38317140/article/details/90038858)<br>
[tolua学习(二)](https://blog.csdn.net/qq_38317140/article/details/90058028)<br>
[tolua学习(三)](https://blog.csdn.net/qq_38317140/article/details/90147397)<br>

