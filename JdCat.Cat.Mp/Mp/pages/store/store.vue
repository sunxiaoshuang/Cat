<template>
	<view class="content">
		<view class="info">
			<uni-swipe-action :options="options" @click="edit($event, 'name')">
				<view class="info-item">
					<text class="info-title">商户名称</text>
					<text class="info-content">{{store.name}}</text>
				</view>
			</uni-swipe-action>
			<view class="info-item">
				<text class="info-title">商户编码</text>
				<text class="info-content">{{store.storeId}}</text>
			</view>
			<uni-swipe-action :options="options" @click="edit($event, 'contact')">
				<view class="info-item">
					<text class="info-title">联系人</text>
					<text class="info-content">{{store.contact}}</text>
				</view>
			</uni-swipe-action>
			<uni-swipe-action :options="options" @click="edit($event, 'mobile')">
				<view class="info-item">
					<text class="info-title">联系电话</text>
					<text class="info-content">{{store.mobile}}</text>
				</view>
			</uni-swipe-action>
			<uni-swipe-action :options="options" @click="editCity()">
				<view class="info-item">
					<text class="info-title">所在地</text>
					<text class="info-content">{{store.province + ' ' + store.city + ' ' + store.area}}</text>
				</view>
			</uni-swipe-action>
			<uni-swipe-action :options="options" @click="edit($event, 'address')">
				<view class="info-item">
					<text class="info-title">详细地址</text>
					<text class="info-content">{{store.address}}</text>
				</view>
			</uni-swipe-action>
			<view class="info-item">
				<text class="info-title">是否暂停营业</text>
				<switch :checked="store.isClose" @change="switchChange($event, 'isClose')" />
			</view>
			<view class="info-item">
				<text class="info-title">是否自动接单</text>
				<switch :checked="store.isAutoReceipt" @change="switchChange($event, 'isAutoReceipt')" />
			</view>
			<view class="info-item">
				<text class="info-title">是否活动同享</text>
				<switch :checked="store.isEnjoymentActivity" @change="switchChange($event, 'isEnjoymentActivity')" />
			</view>
			<uni-swipe-action :options="options" @click="edit($event, 'discountQuantity', 'number')">
				<view class="info-item">
					<text class="info-title">享受折扣数量</text>
					<text class="info-content">{{store.discountQuantity}}</text>
				</view>
			</uni-swipe-action>
			<uni-swipe-action :options="options" @click="edit($event, 'lng', 'number')">
				<view class="info-item">
					<text class="info-title">商铺经度</text>
					<text class="info-content">{{store.lng}}</text>
				</view>
			</uni-swipe-action>
			<uni-swipe-action :options="options" @click="edit($event, 'lat', 'number')">
				<view class="info-item">
					<text class="info-title">商铺纬度</text>
					<text class="info-content">{{store.lat}}</text>
				</view>
			</uni-swipe-action>
			<uni-swipe-action :options="options" @click="edit($event, 'range', 'number')">
				<view class="info-item">
					<text class="info-title">配送范围</text>
					<text class="info-content">{{store.range + ' km'}}</text>
				</view>
			</uni-swipe-action>
			<uni-swipe-action :options="options" @click="edit($event, 'freight', 'number')">
				<view class="info-item">
					<text class="info-title">外卖运费</text>
					<text class="info-content">{{'￥' + store.freight}}</text>
				</view>
			</uni-swipe-action>
			<uni-swipe-action :options="options" @click="edit($event, 'minAmount', 'number')">
				<view class="info-item">
					<text class="info-title">起送金额</text>
					<text class="info-content">{{'￥' + store.minAmount}}</text>
				</view>
			</uni-swipe-action>
			<uni-swipe-action :options="options" @click="editTime()">
				<view class="info-item">
					<text class="info-title">营业时间</text>
					<text class="info-content">{{businessTime}}</text>
				</view>
			</uni-swipe-action>
			<uni-swipe-action :options="options" @click="editDelivery()">
				<view class="info-item">
					<text class="info-title">配送方式</text>
					<text class="info-content">{{serviceProvider}}</text>
				</view>
			</uni-swipe-action>
			<uni-swipe-action :options="options" @click="edit($event, 'description')">
				<view class="info-item">
					<text class="info-title">公告</text>
					<text class="info-content">{{store.description}}</text>
				</view>
			</uni-swipe-action>
		</view>
		<view class="pop" v-bind:style="{display: isShowEdit ? 'block' : 'none'}">
			<view class="pop-shadow"></view>
			<view class="pop-container">
				<view class="pop-content">
					<view class="pop-info">
						<input class="uni-input" :focus="editFocus" v-model="inputContent" placeholder="请输入修改内容..." style="border: 1upx solid #ccc;text-align: center; margin: 40upx;" />
					</view>
					<view class="pop-operate">
						<button type="default" @click="cancelEdit()">取消</button>
						<button type="primary" plain="true" @click="submitEdit()">确定</button>
					</view>
				</view>
			</view>
		</view>
		<w-picker mode="region" @confirm="onConfirmCity" ref="picker" themeColor="#f00"></w-picker>
		<w-picker mode="selector" @confirm="onConfirmDelivery" :selectList="deliverys" ref="picker1" themeColor="#f00"></w-picker>
		<view class="pop" v-bind:style="{display: isShowTime ? 'block' : 'none'}">
			<view class="pop-shadow"></view>
			<view class="pop-container">
				<view class="pop-content">
					<view class="pop-info">
						<view class="time-selector">
							<view class="time-item">
								<text>营业时段1：</text>
								<picker mode="time" :value="timeObj.startTime1" start="00:00" end="23:59" @change="bindTimeChange($event, 'startTime1')">
									<view class="uni-input">{{timeObj.startTime1}}</view>
								</picker>
								<text style="width: 50upx; text-align: center;"> - </text>
								<picker mode="time" :value="timeObj.endTime1" start="00:00" end="23:59" @change="bindTimeChange($event, 'endTime1')">
									<view class="uni-input">{{timeObj.endTime1}}</view>
								</picker>
							</view>
							<view class="time-item">
								<text>营业时段2：</text>
								<picker mode="time" :value="timeObj.startTime2" start="00:00" end="23:59" @change="bindTimeChange($event, 'startTime2')">
									<view class="uni-input">{{timeObj.startTime2}}</view>
								</picker>
								<text style="width: 50upx; text-align: center;"> - </text>
								<picker mode="time" :value="timeObj.endTime2" start="00:00" end="23:59" @change="bindTimeChange($event, 'endTime2')">
									<view class="uni-input">{{timeObj.endTime2}}</view>
								</picker>
							</view>
							<view class="time-item">
								<text>营业时段3：</text>
								<picker mode="time" :value="timeObj.startTime3" start="00:00" end="23:59" @change="bindTimeChange($event, 'startTime3')">
									<view class="uni-input">{{timeObj.startTime3}}</view>
								</picker>
								<text style="width: 50upx; text-align: center;"> - </text>
								<picker mode="time" :value="timeObj.endTime3" start="00:00" end="23:59" @change="bindTimeChange($event, 'endTime3')">
									<view class="uni-input">{{timeObj.endTime3}}</view>
								</picker>
							</view>
							<text style="text-align: center; font-size: 80%; color: #ea7042;">注：不用的时段请选择：00:00</text>
						</view>
					</view>
					<view class="pop-operate">
						<button type="default" @click="cancelEditTime()">取消</button>
						<button type="primary" plain="true" @click="submitEditTime()">确定</button>
					</view>
				</view>
			</view>
		</view>
	</view>
</template>

<script>
	import uniSwipeAction from "@/components/uni-swipe-action/uni-swipe-action.vue";
	import wPicker from "@/components/w-picker/w-picker.vue";
	import helper from '../../common/helper.js';
	const services = {0: "未知", 1: "商家自送", 2: "达达配送", 3: "美团配送", 4: "蜂鸟配送", 5: "点我达配送", 6: "一城飞客配送"};
	export default {
		components: {
			uniSwipeAction,
			wPicker
		},
		data() {
			return {
				options: [{
					text: '修改',
					style: {
						backgroundColor: '#ea7042'
					}
				}],
				businessId: 0,
				store: {},
				deliverys: [{
					label: "商家自送",
					value: "1"
				}, {
					label: "达达配送",
					value: "2"
				}, {
					label: "美团配送",
					value: "3"
				}, {
					label: "蜂鸟配送",
					value: "4"
				}, {
					label: "点我达配送",
					value: "5"
				}, {
					label: "一城飞客配送",
					value: "6"
				}],

				isShowEdit: false,
				editField: null,
				editType: null,
				inputContent: '',
				editFocus: false,

				isShowTime: false,
				timeObj: {
					startTime1: '',
					startTime2: '',
					startTime3: '',
					endTime1: '',
					endTime2: '',
					endTime3: '',
				}
			}
		},
		onLoad(e) {
			var self = this;
			this.businessId = e.id || 1;
			helper.request({
				url: "/mana/getstore/" + this.businessId,
				success(res) {
					self.store = res.data;
					self.timeObj.startTime1 = res.data.businessStartTime || "00:00";
					self.timeObj.endTime1 = res.data.businessEndTime || "00:00";
					self.timeObj.startTime2 = res.data.businessStartTime2 || "00:00";
					self.timeObj.endTime2 = res.data.businessEndTime2 || "00:00";
					self.timeObj.startTime3 = res.data.businessStartTime3 || "00:00";
					self.timeObj.endTime3 = res.data.businessEndTime3 || "00:00";
				}
			});
		},
		computed: {
			serviceProvider() {
				return services[this.store.serviceProvider];
			},
			businessTime(){
				var time = '';
				if(this.store.businessStartTime && this.store.businessEndTime){
					time += this.store.businessStartTime + '-' + this.store.businessEndTime
				}
				if(this.store.businessStartTime2 && this.store.businessEndTime2){
					if(time.length > 0) time += ',';
					time += this.store.businessStartTime2 + '-' + this.store.businessEndTime2
				}
				if(this.store.businessStartTime3 && this.store.businessEndTime3){
					if(time.length > 0) time += ',';
					time += this.store.businessStartTime3 + '-' + this.store.businessEndTime3
				}
				return time;
			}
		},
		methods: {
			edit(e, name, type) {
				this.editField = name;
				this.editType = type;
				this.isShowEdit = true;
				this.inputContent = this.store[name];
				this.editFocus = true;
				
				
			},
			cancelEdit() {
				this.isShowEdit = false;
				this.inputContent = "";
				this.editFocus = false;
			},
			submitEdit() {
				var self = this;
				if (this.editType === "number" && isNaN(+this.inputContent)) {
					uni.showToast({
						title: '请输入正确的数字',
						icon: 'none'
					});
					return;
				}
				uni.showLoading();
				var postData = {
					id: this.businessId
				};
				var field = this.editField.slice(0, 1).toUpperCase() + this.editField.slice(1);
				var input = this.inputContent;
				postData[this.editField] = input;
				helper.request({
					url: `/mana/business?field=${field}`,
					method: "PUT",
					data: postData,
					success(res) {
						uni.hideLoading();
						uni.showToast({
							title: "修改成功",
							icon: "success"
						});
						self.store[self.editField] = input;
					},
					fail(err) {
						uni.hideLoading();
						uni.showToast({
							title: err,
							icon: "none"
						})
					}
				});
				this.cancelEdit();
			},

			editCity() {
				this.$refs.picker.show();
			},
			onConfirmCity(e) {
				var arr = e.checkArr,
					self = this;
				var postData = {
					id: this.businessId,
					province: arr[0],
					city: arr[1],
					area: arr[2]
				};
				helper.request({
					url: `/mana/city`,
					method: "PUT",
					data: postData,
					success(res) {
						uni.showToast({
							title: "修改成功",
							icon: "success"
						});
						self.store.province = arr[0];
						self.store.city = arr[1];
						self.store.area = arr[2];
					},
					fail(err) {
						uni.showToast({
							title: err,
							icon: "none"
						})
					}
				});
			},
		
			editDelivery(){
				this.$refs.picker1.show();
			},
			onConfirmDelivery(e){
				var obj = e.checkArr, self = this;
				helper.request({
					url: `/mana/business?field=ServiceProvider`,
					method: "PUT",
					data: {id: this.businessId, serviceProvider: +obj.value},
					success(res){
						uni.showToast({
							title: "修改成功",
							icon: "success"
						});
						self.store.serviceProvider = +obj.value;
					},
					fail(err) {
						uni.showToast({
							title: err,
							icon: "none"
						})
					}
				});
			},
			
			editTime(){
				this.isShowTime = true;
			},
			bindTimeChange(a, b){
				this.timeObj[b] = a.detail.value;
			},
			cancelEditTime(){
				this.isShowTime = false;
			},
			submitEditTime(){
				var self = this, postData = {
					id: this.businessId,
					businessStartTime: this.timeObj.startTime1 || '00:00',
					businessEndTime: this.timeObj.endTime1 || '00:00',
					businessStartTime2: this.timeObj.startTime2 || '00:00',
					businessEndTime2: this.timeObj.endTime2 || '00:00',
					businessStartTime3: this.timeObj.startTime3 || '00:00',
					businessEndTime3: this.timeObj.endTime3 || '00:00'
				};
				if(postData.businessEndTime === "00:00") {
					uni.showToast({
						title: "营业时间1必须为有效时间",
						icon: "none"
					});
					return;
				}
				if(postData.businessStartTime > postData.businessEndTime || postData.businessStartTime2 > postData.businessEndTime2 ||postData.businessStartTime3 > postData.businessEndTime3) {
					uni.showToast({
						title: "营业结束时间不能小于营业开始时间",
						icon: "none"
					});
					return;
				}
				if(postData.businessStartTime2 === "00:00" && postData.businessEndTime2 === "00:00") {
					postData.businessStartTime2 = postData.businessEndTime2 = null;
				}
				if(postData.businessStartTime3 === "00:00" && postData.businessEndTime3 === "00:00") {
					postData.businessStartTime3 = postData.businessEndTime3 = null;
				}
				helper.request({
					url: `/mana/time`,
					data: postData,
					method: "PUT",
					success(){
						uni.showToast({
							title: "修改成功",
							icon: "success"
						});
						helper.extend(self.store, postData);
					}
				});
				this.cancelEditTime();
			},
			
			switchChange($event, name) {
				var postData = {id: this.businessId};
				postData[name] = $event.detail.value;
				helper.request({
					url: `/mana/business?field=${helper.firstLetterUpper(name)}`,
					method: "PUT",
					data: postData,
					success(res) {
						uni.showToast({
							title: "修改成功",
							icon: "success"
						});
					}
				});
			}
			
		}
	}
</script>

<style>
	.info {
		display: flex;
		flex-flow: column;
		margin-bottom: 500upx;
	}

	.info .info-item {
		display: flex;
		flex-flow: row nowrap;
		justify-content: space-between;
		border-bottom: 1upx solid #ccc;
		padding: 15upx;
		font-size: 32upx;
	}

	.info-title {
		width: 200upx;
		white-space: nowrap;
	}

	.info-content {
		max-width: 500upx;
		text-align: right;

	}

	.pop {
		position: fixed;
		top: 0;
		left: 0;
		width: 100%;
		height: 100%;
		display: none;
	}

	.pop-shadow {
		background: #000000;
		width: 100%;
		height: 100%;
		opacity: .3;
	}

	.pop-container {
		position: fixed;
		top: 0;
		left: 0;
		width: 100%;
		height: 100%;
		display: flex;
		justify-content: center;
		align-items: center;
	}

	.pop-content {
		height: auto;
		min-height: 400upx;
		width: 80%;
		background: #fff;
		border-radius: 10upx;
		padding: 10upx;
		box-shadow: 0 0 10upx 0 #000;
		display: flex;
		flex-direction: column;
	}

	.pop-info {
		flex-grow: 1;
	}

	.pop-operate {
		height: 100upx;
		text-align: center;
	}

	.pop-operate button {
		display: inline;
		height: 80upx;
		margin-left: 40upx;
		padding: 10upx 20upx;
	}
	
	.time-selector {
		display: flex;
		flex-direction: column;
	}
	.time-item {
		display: flex;
		flex-flow: row nowrap;
		justify-content: center;
		padding: 10upx;
		border-bottom: 1upx solid #ccc;
	}
</style>
