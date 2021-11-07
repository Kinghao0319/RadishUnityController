# RadishUnityController
Unity动画控制器结合Python实时语音识别

应用于“拔萝卜”动画场景

### 使用流程

1. 配置const.py中API的ID, KEY
2. 把代码中静态txt路径设置为一致

3. Python与Unity分别执行

Python执行后会将输入的语音实时转为文字**写入**指定静态文件

Unity隔一段时间**读取**静态文件获得文字内容，并根据文字控制动画