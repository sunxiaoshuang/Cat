<template>
	<view>
		<uni-list>
			<template v-for="order in orders">
				<uni-swipe-action v-if="isShow(order)" :options="options" @click="handle($event, order)" :key="order.id">
					<uni-list-item :title="title(order)" :show-arrow="false" :showBadge="true" :badgeText="badge(order)" :badgeType="badgeType(order)"></uni-list-item>
				</uni-swipe-action>
				<uni-list-item v-else :key="order.id" :title="title(order)" :show-arrow="false" :showBadge="false"></uni-list-item>
			</template>
		</uni-list>
	</view>
</template>

<script>
	import uniList from '@/components/uni-list/uni-list.vue'
	import uniListItem from '@/components/uni-list-item/uni-list-item.vue'
	import uniSwipeAction from "@/components/uni-swipe-action/uni-swipe-action.vue"
	import helper from '../../common/helper.js'
	const validStatus = [1, 4, 8, 16]
	export default {
		name: 'selfTakeOrder',
		components: {
			uniList,
			uniListItem,
			uniSwipeAction
		},
		data() {
			return {
				businessId: 0,
				orders: [],
				options: [{
						text: '已领取',
						style: {
							backgroundColor: '#dd524d'
						}
					},
					{
						text: '已出餐',
						style: {
							backgroundColor: '#007aff'
						}
					}
				]
			}
		},
		computed: {
			badge() {
				return order => {
					if (order.deliveryMode === 2) {
						if ([1, 4].indexOf(order.status) > -1) {
							return '制作中'
						} else {
							return '已通知'
						}
					}
					return ''
				}
			},
			badgeType() {
				return order => {
					return [1, 4].indexOf(order.status) > -1 ? 'error' : 'success'
				}
			},
			title() {
				return order => `今日单号：${order.identifier}，自提人：${order.receiverName}`
			}
		},
		filters: {
			isShow(order) {
				return validStatus.indexOf(order.status) > -1
			}
		},
		methods: {
			onLoad(e) {
				this.businessId = e.id
				this.loadData()
			},
			onPullDownRefresh() {
				this.loadData()
			},
			loadData() {
				let self = this
				let res = helper.request({
					url: "/mana/selfTakeOrders/" + this.businessId,
					success(res) {
						self.orders = res.data
						uni.stopPullDownRefresh()
					}
				})
			},
			handle(e, order) {
				if (e.text === '已出餐') {
					this.notify(order)
				} else {
					this.finish(order)
				}
			},
			isShow(order) {
				return validStatus.indexOf(order.status) > -1
			},

			/**
			 * 出餐提醒
			 */
			notify(order) {
				helper.request({
					url: `/mana/sendMsg/${order.id}`,
					success() {
						order.status = 8
						uni.showToast({
							title: '通知成功'
						})
					}
				})
			},
			/**
			 * 完成订单
			 */
			finish(order) {
				let self = this
				if (validStatus.indexOf(order.status) === -1) {
					uni.showToast({
						title: '订单已被领取',
						icon: 'none'
					})
					return
				}
				helper.request({
					url: `/mana/takenAway/${order.id}`,
					method: 'put',
					success() {
						order.status = 64
						uni.showToast({
							title: '领取成功'
						})
					}
				})
			}

		}
	}
</script>

<style>

</style>
