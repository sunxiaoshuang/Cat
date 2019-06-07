<template>
	<view class="page">
		<view class="store">
			<view class="uni-title uni-common-pl store-title">当前选择商户：</view>
			<view class="uni-list">
				<view class="uni-list-cell">
					<view class="uni-list-cell-db">
						<picker @change="bindPickerChange" mode="selector" :value="index" :range="array" range-key="name">
							<view class="uni-input">{{array[index].name}}</view>
						</picker>
					</view>
				</view>
			</view>
		</view>
				
		
		<view class="example">
			<view class="example-title">{{title}}</view>
			<uni-grid :options="data1" @click="onClick" />
		</view>
	</view>
</template>

<script>
	import uniGrid from '@/components/uni-grid/uni-grid.vue'
    import config from '../../config.js'
    import helper from '../../common/helper.js'
	export default {
		components: {
			uniGrid
		},
		data() {
			return {
				openid: '', 
				title: '功能',
				data1: [
					{
						image: '/static/shangdian.png',
						text: '商户信息',
						url: '/pages/store/store'
					},
					{
						image: '/static/menu.png',
						text: '我的商品',
						url: '/pages/product/product'
					},
					// {
					// 	image: '/static/yingye-cny.png',
					// 	text: '营业统计',
					// 	url: '/pages/orderTotal/orderTotal'
					// }
				],
				index: 0,
				array: [{name: '', id: 0}],
				businessId: 0
				
			} 
		},
		onLoad(e) {
			var self = this;
			this.openid = e.openid || config.openid;
			helper.request({
				url: '/mana/business?openid=' + this.openid,
				success: function(res){
					if(!res.data) {
						uni.showToast({
							title: '未绑定任何商户',
							icon: 'none'
						})
						return;
					}
					self.array = res.data;
					self.businessId = res.data[0].id;
				},
			})
		},
		methods: {
			onClick(e){
				if(this.businessId <= 0){
					uni.showToast({
						title: '请先在网页后台绑定微信用户',
						icon: 'none'
					})
					return;
				}
				var url = this.data1[e.index].url;
				uni.navigateTo({
					url: `${url}?id=` + this.businessId
				});
			},
			bindPickerChange(e){
				this.index = e.target.value
				this.businessId = this.array[this.index].id;
			}
		}
	}
</script>

<style>
	page {
		display: flex;
		flex-direction: column;
		box-sizing: border-box;
		background-color: #fff
	}

	view {
		font-size: 28upx;
		line-height: inherit
	}
	
	.store {
		display: flex;
		flex-direction: row;
	}

	.store, .example {
		padding: 0 30upx 30upx
	}

	.store-title, .example-title {
		font-size: 32upx;
		line-height: 32upx;
		color: #777;
		margin: 40upx 0;
		position: relative;
		border-left: 10upx solid #ea7042;
		padding-left: 10upx;
	}

	.example-body {
		padding: 0 40upx
	}

	.grid-view {
		/* #ifdef H5 */
		padding: 0 0.5px;
		/* #endif */
		box-sizing: border-box;
	}
	
	.store-title {
		margin: 20upx 0;
	}
	.uni-list {
		margin: 16upx 0;
	}
</style>