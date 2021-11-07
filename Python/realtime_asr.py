# -*- coding: utf-8 -*-
"""
实时流式识别
需要安装websocket-client库
使用方式 python realtime_asr.py 16k-0.pcm
"""
import websocket

import threading
import time
import uuid
import json
import logging
import sys

from pyaudio import PyAudio, paInt16

import const

# if len(sys.argv) < 2:
#     pcm_file = "16k-0.pcm"
# else:
#     pcm_file = sys.argv[1]
#
# logger = logging.getLogger()

"""

1. 连接 ws_app.run_forever()
2. 连接成功后发送数据 on_open()
2.1 发送开始参数帧 send_start_params()
2.2 发送音频数据帧 send_audio()
2.3 库接收识别结果 on_message()
2.4 发送结束帧 send_finish()
3. 关闭连接 on_close()

库的报错 on_error()
"""

framerate = 16000
NUM_SAMPLES = 2000
channels = 1
sampwidth = 2
TIME = 1000
audio_buf=[]
audio_len=0
index=0
buf_time = 2

import time
import threading


class MyThread1(threading.Thread):
    def run(self):
        print("record_thread start...")
        record_thread()
        print("record_thread exit")


def record_thread():
    pa = PyAudio()
    stream = pa.open(format=paInt16, channels=1,
                     rate=framerate, input=True,
                     frames_per_buffer=NUM_SAMPLES)

    total_time = 0
    global audio_len
    while total_time < TIME * 8:
        string_audio_data = stream.read(NUM_SAMPLES)
        #print(string_audio_data)
        for ii in string_audio_data:
            audio_buf.append(ii)
        audio_len+=len(string_audio_data)
        total_time += 1
        if(not thread2.is_alive()):
            sys.exit()


def send_audio2(ws):
    print("sending audio to recognize!")
    chunk_ms = 160  # 160ms的录音
    chunk_len = int(16000 * 2 / 1000 * chunk_ms)

    index = 0
    #logger.info("send_audio start")
    while True:
        end = index + chunk_len
        curLen=len(audio_buf)
        if end >= curLen:
            #print(curLen)
            continue
        body = audio_buf[index:end]
        #logger.debug("try to send audio length {}, from bytes [{},{})".format(len(body), index, end))
        ws.send(body, websocket.ABNF.OPCODE_BINARY)
        index = end
        time.sleep(chunk_ms / 1000.0)  # ws.send 也有点耗时，这里没有计算


def send_start_params(ws):
    """
    开始参数帧
    :param websocket.WebSocket ws:
    :return:
    """
    req = {
        "type": "START",
        "data": {
            "appid": const.APPID,  # 网页上的appid
            "appkey": const.APPKEY,  # 网页上的appid对应的appkey
            "dev_pid": const.DEV_PID,  # 识别模型
            "cuid": "yourself_defined_user_id",  # 随便填不影响使用。机器的mac或者其它唯一id，百度计算UV用。
            "sample": 16000,  # 固定参数
            "format": "pcm"  # 固定参数
        }
    }
    body = json.dumps(req)
    ws.send(body, websocket.ABNF.OPCODE_TEXT)
    #logger.info("send START frame with params:" + body)



# def send_audio(ws):
#     """
#     发送二进制音频数据，注意每个帧之间需要有间隔时间
#     :param  websocket.WebSocket ws:
#     :return:
#     """
#     chunk_ms = 160  # 160ms的录音
#     chunk_len = int(16000 * 2 / 1000 * chunk_ms)
#     with open(pcm_file, 'rb') as f:
#         pcm = f.read()
#
#     index = 0
#     total = len(pcm)
#     logger.info("send_audio total={}".format(total))
#     while index < total:
#         end = index + chunk_len
#         if end >= total:
#             # 最后一个音频数据帧
#             end = total
#         body = pcm[index:end]
#         #logger.debug("try to send audio length {}, from bytes [{},{})".format(len(body), index, end))
#         ws.send(body, websocket.ABNF.OPCODE_BINARY)
#         index = end
#         time.sleep(chunk_ms / 1000.0)  # ws.send 也有点耗时，这里没有计算


def send_finish(ws):
    """
    发送结束帧
    :param websocket.WebSocket ws:
    :return:
    """
    req = {
        "type": "FINISH"
    }
    body = json.dumps(req)
    ws.send(body, websocket.ABNF.OPCODE_TEXT)
    #logger.info("send FINISH frame")


def send_cancel(ws):
    """
    发送取消帧
    :param websocket.WebSocket ws:
    :return:
    """
    req = {
        "type": "CANCEL"
    }
    body = json.dumps(req)
    ws.send(body, websocket.ABNF.OPCODE_TEXT)
    #logger.info("send Cancel frame")


def on_open(ws):
    """
    连接后发送数据帧
    :param  websocket.WebSocket ws:
    :return:
    """

    def run(*args):
        """
        发送数据帧
        :param args:
        :return:
        """
        send_start_params(ws)
        send_audio2(ws)#改成我的了
        send_finish(ws)
        #logger.debug("thread terminating")

    threading.Thread(target=run).start()





def on_message(ws, data):
    """
    接收服务端返回的消息
    :param ws:
    :param message: json格式，自行解析
    :return:
    """
    #logger.info("Response: " + message)
    message=json.loads(data)
    #global buf_str
    cur=message["result"]
    fo = open("a.txt", "a",encoding='utf-8')
    fo.write(cur+'\n')
    fo.close()
    print(cur)
    if("退出" in cur):
        sys.exit()
    # last_len=len(buf_str)
    # cur_len=len(cur)
    # logger.info(cur+" "+buf_str)
    # if(cur_len>last_len):
    #     print(cur[last_len:cur_len],end="")
    #     if(message["type"]=="FIN_TEXT"):
    #         print()
    # globals()["buf_str"]=cur



def on_error(ws, error):
    """
    库的报错，比如连接超时
    :param ws:
    :param error: json格式，自行解析
    :return:
        """
    #logger.error("error: " + str(error))



def on_close(ws):
    """
    Websocket关闭
    :param websocket.WebSocket ws:
    :return:
    """
    #logger.info("ws close ...")
    print("进程2关闭")
    # ws.close()

class MyThread2(threading.Thread):
    def run(self):
        # logging.basicConfig(format='[%(asctime)-15s] [%(funcName)s()][%(levelname)s] %(message)s')
        # logger.setLevel(logging.INFO)  # 调整为logging.INFO，日志会少一点
        # logger.info("begin")
        # # websocket.enableTrace(True)
        uri = const.URI + "?sn=" + str(uuid.uuid1())
        # logger.info("uri is " + uri)

        ws_app = websocket.WebSocketApp(uri,
                                        on_open=on_open,  # 连接建立后的回调
                                        on_message=on_message,  # 接收消息的回调
                                        on_error=on_error,  # 库遇见错误的回调
                                        on_close=on_close)  # 关闭后的回调
        ws_app.keep_running=False
        ws_app.run_forever()
        ws_app.close()


# if __name__ == "__main__":
#     thread1 = MyThread1()
#     thread2 = MyThread2()
#     thread1.start()
#     thread2.start()
#     thread1.join()
#     thread2.join()
#     print("EXIT!")

thread1 = MyThread1()
thread2 = MyThread2()
thread1.start()
thread2.start()
thread1.join()
thread2.join()


